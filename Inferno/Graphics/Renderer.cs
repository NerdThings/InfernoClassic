using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Inferno.Graphics.Text;

namespace Inferno.Graphics
{
    /// <summary>
    /// The sort mode for the renderer
    /// </summary>
    public enum RenderSortMode
    {
        /// <summary>
        /// Draw order
        /// </summary>
        Immediate,

        /// <summary>
        /// Sorted by the depth argument
        /// </summary>
        Depth
    }

    /// <summary>
    /// Graphics rendering and batching
    /// </summary>
    public partial class Renderer : IDisposable
    {
        #region Private Fields
        
        /// <summary>
        /// The graphics device
        /// </summary>
        private readonly GraphicsDevice _graphicsDevice;
        
        /// <summary>
        /// The translation matrix
        /// </summary>
        private Matrix _matrix;
        
        /// <summary>
        /// Whether or not the renderer is active
        /// </summary>
        private bool _rendering;
        
        /// <summary>
        /// List of items in the batch to be renderer
        /// </summary>
        private List<Renderable> _renderList;
        
        /// <summary>
        /// The batch sort mode
        /// </summary>
        private RenderSortMode _sortMode;
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Create a new renderer
        /// </summary>
        /// <param name="graphicsDevice">The current graphics device</param>
        public Renderer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _matrix = Matrix.Identity;
        }
        
        #endregion

        #region Public Methods
        
        #region Begin and End
        
        /// <summary>
        /// Begin a drawing batch
        /// </summary>
        /// <param name="sortMode">Batch sort mode</param>
        /// <param name="translationMatrix">Translation matrix for batch</param>
        public void Begin(RenderSortMode sortMode = RenderSortMode.Immediate, Matrix? translationMatrix = null)
        {
            //Prepare batch list
            if (_renderList == null)
                _renderList = new List<Renderable>();

            //Clear batch list
            _renderList.Clear();
            
            //Mark as rendering
            _rendering = true;
            
            //Set sortmode and matrix
            _sortMode = sortMode;
            _matrix = translationMatrix ?? Matrix.Identity;

            //Initiate renderer
            BeginRender(_matrix);
        }

        /// <summary>
        /// End a drawing batch
        /// </summary>
        public void End()
        {
            //Get the render list
            var renderables =  _renderList;

            //Sort the list if we need to
            if (_sortMode == RenderSortMode.Depth)
                renderables = _renderList.OrderBy(o => o.Depth).ToList();

            //Render each batch item
            foreach (var renderable in renderables)
            {
                //Render
                Render(renderable);

                //Dispose if we have to
                if (!renderable.Dispose)
                    continue;

                renderable.Texture?.Dispose();
                renderable.RenderTarget?.Dispose();
                renderable.Font?.Dispose();
            }

            //Mark as render finished
            _rendering = false;
        }
        
        #endregion
        
        #region Primitives
        
        /// <summary>
        /// Draw circle
        /// </summary>
        /// <param name="position">Position to draw at</param>
        /// <param name="radius">Circle radius</param>
        /// <param name="color">Color to draw</param>
        /// <param name="depth">Depth to draw at</param>
        /// <param name="filled">Whether or not it is filled</param>
        /// <param name="lineWidth">Line width (only affects non filled)</param>
        /// <param name="circlePrecision">Lines/Points per circle</param>
        public void DrawCircle(Vector2 position, int radius, Color color, float depth = 0f, bool filled = true, int lineWidth = 1, int circlePrecision = 24)
        {
            var destRectangle = new Rectangle
            {
                X = (int)position.X,
                Y = (int)position.Y
            };

            _renderList.Add(new Renderable
                {
                    Type = filled ? RenderableType.FilledCircle : RenderableType.Circle,
                    Depth = depth,
                    Color = color,
                    DestinationRectangle = destRectangle,
                    Radius = radius,
                    Precision = circlePrecision,
                    LineWidth = lineWidth
                }
            );
        }
        
        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="a">Point a</param>
        /// <param name="b">Point b</param>
        /// <param name="color">Color of line</param>
        /// <param name="lineWidth">Line width</param>
        /// <param name="depth">Line depth</param>
        public void DrawLine(Vector2 a, Vector2 b, Color color, int lineWidth = 1, float depth = 0f)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            var vertexArray = new[] {a, b};
            
            DrawLines(vertexArray, color, lineWidth, depth);
        }
        
        /// <summary>
        /// Draw a line array
        /// </summary>
        /// <param name="points">Points on the line</param>
        /// <param name="color">Color of line</param>
        /// <param name="lineWidth">Line width</param>
        /// <param name="depth">Depth to draw at</param>
        public void DrawLines(Vector2[] points, Color color, int lineWidth = 1, float depth = 0f)
        {
            _renderList.Add(new Renderable
                {
                    Type = RenderableType.Lines,
                    Depth = depth,
                    Color = color,
                    Verticies = points,
                    LineWidth = lineWidth
                }
            );
        }
        
        /// <summary>
        /// Draw Rectangle
        /// </summary>
        /// <param name="rect">Rectangle to draw</param>
        /// <param name="color">Color to draw</param>
        /// <param name="depth">Depth to draw at</param>
        /// <param name="filled">Whether or not it is filled</param>
        /// <param name="lineWidth">Line width (only affects non-filled)</param>
        public void DrawRectangle(Rectangle rect, Color color, float depth = 0f, bool filled = true, int lineWidth = 1)
        {
            if (filled)
            {
                _renderList.Add(new Renderable
                    {
                        Type = RenderableType.Rectangle,
                        Depth = depth,
                        Color = color,
                        DestinationRectangle = rect
                        //Rotation = rotation
                    }
                );
            }
            else
            {
                //Draw the outline
                var vertexArray = new[]
                {
                    new Vector2(rect.Left, rect.Top),
                    new Vector2(rect.Right, rect.Top),
                    new Vector2(rect.Right, rect.Top),
                    new Vector2(rect.Right, rect.Bottom),
                    new Vector2(rect.Right, rect.Bottom),
                    new Vector2(rect.Left, rect.Bottom),
                    new Vector2(rect.Left, rect.Bottom),
                    new Vector2(rect.Left, rect.Top),
                };
                
                DrawLines(vertexArray, color, lineWidth, depth);
            }
        }
        
        #endregion
        
        #region RenderTargets
        
        /// <summary>
        /// Draw Render Target
        /// </summary>
        /// <param name="target">Render target</param>
        /// <param name="position">Position</param>
        /// <param name="color">Color modifier</param>
        /// <param name="disposeAfterDraw">Dispose target after draw</param>
        public void Draw(RenderTarget target, Vector2 position, Color color, bool disposeAfterDraw = false)
        {
            Draw(target, new Rectangle((int) position.X, (int) position.Y, target.Width, target.Height), color,
                disposeAfterDraw);
        }

        /// <summary>
        /// Draw Render Target
        /// </summary>
        /// <param name="target">Render target</param>
        /// <param name="destRectangle">Destination rectangle</param>
        /// <param name="color">Color modifier</param>
        /// <param name="disposeAfterDraw">Dispose target after draw</param>
        public void Draw(RenderTarget target, Rectangle destRectangle, Color color, bool disposeAfterDraw = false)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            _renderList.Add(new Renderable
                {
                    Type = RenderableType.RenderTarget,
                    RenderTarget = target,
                    Color = color,
                    DestinationRectangle = destRectangle,
                    Dispose = disposeAfterDraw
                }
            );
        }
        
        #endregion
        
        #region Text
        
        /// <summary>
        /// Draw text
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="position">Position to draw</param>
        /// <param name="font">Font to draw in</param>
        public void DrawText(string text, Vector2 position, Font font)
        {
            DrawText(text, position, font, Color.Black);
        }

        /// <summary>
        /// Draw text
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="position">Position to draw</param>
        /// <param name="font">Font to draw in</param>
        /// <param name="color">Text color</param>
        /// <param name="depth">Depth to draw at</param>
        public void DrawText(string text, Vector2 position, Font font, Color color, float depth = 0)
        {
            DrawText(text, position, font, color, depth, new Vector2(0, 0), 0);
        }

        /// <summary>
        /// Draw text
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="position">Position to draw</param>
        /// <param name="font">Font to draw in</param>
        /// <param name="color">Text color</param>
        /// <param name="depth">Depth to draw at</param>
        /// <param name="origin">Origin of text</param>
        /// <param name="rotation">Text Rotation</param>
        /// <param name="disposeAfterDraw">Dispose font after drawing</param>
        public void DrawText(string text, Vector2 position, Font font, Color color, float depth, Vector2 origin, double rotation, bool disposeAfterDraw = false)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            var size = font.MeasureString(text);

            var destRectangle = new Rectangle((int) position.X, (int) position.Y, (int)size.X, (int)size.Y);

            _renderList.Add(new Renderable
                {
                    Type = RenderableType.Text,
                    Font = font,
                    Text = text,
                    DestinationRectangle = destRectangle,
                    Color = color,
                    Origin = origin,
                    Rotation =  rotation,
                    Depth = depth,
                    Dispose = disposeAfterDraw
                }
            );
        }
        
        #endregion
        
        #region Textures
        
        /// <summary>
        /// Draw a texture
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="position">Position</param>
        public void Draw(Texture2D texture, Vector2 position)
        {
            Draw(texture, position, Color.White);
        }

        /// <summary>
        /// Draw texture
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="position">Position</param>
        /// <param name="color">Color modifier</param>
        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            Draw(texture, position, color, 0f);
        }

        /// <summary>
        /// Draw texture
        /// </summary>
        /// <param name="texture">Textyre to draw</param>
        /// <param name="position">Position</param>
        /// <param name="color">Color modifier</param>
        /// <param name="depth">Depth to draw at</param>
        public void Draw(Texture2D texture, Vector2 position, Color color, float depth)
        {
            Draw(texture, color, depth, position, null, new Vector2(0, 0));
        }

        /// <summary>
        /// Draw texture
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="color">Color modifier</param>
        /// <param name="depth">Depth to draw at</param>
        /// <param name="position">Position</param>
        /// <param name="sourceRectangle">Source rectangle</param>
        /// <param name="origin">Origin</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="disposeAfterDraw">Dispose texture after draw</param>
        public void Draw(Texture2D texture, Color color, float depth, Vector2 position, Rectangle? sourceRectangle, Vector2 origin, double rotation = 0, bool disposeAfterDraw = false)
        {
            var destinationRectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);

            if (sourceRectangle.HasValue)
            {
                destinationRectangle.Width = sourceRectangle.Value.Width;
                destinationRectangle.Height = sourceRectangle.Value.Height;
            }

            Draw(texture, color, depth, destinationRectangle, sourceRectangle, origin, rotation, disposeAfterDraw);
        }

        /// <summary>
        /// Draw texture
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="color">Color modifier</param>
        /// <param name="depth">Depth to draw at</param>
        /// <param name="destinationRectangle">Position and size to draw at</param>
        /// <param name="sourceRectangle">Source rectangle</param>
        /// <param name="origin">Origin</param>
        /// <param name="rotation">Rotation</param>
        /// <param name="disposeAfterDraw">Dispose texture after draw</param>
        public void Draw(Texture2D texture, Color color, float depth, Rectangle destinationRectangle, Rectangle? sourceRectangle, Vector2 origin, double rotation = 0, bool disposeAfterDraw = false)
        {
            if (!_rendering)
                throw new Exception("Cannot call Draw(...) before calling BeginRender.");

            if (texture == null)
                throw new Exception("Texture cannot be null.");

            var pos = new Vector2(destinationRectangle.X, destinationRectangle.Y);
            destinationRectangle.X = (int)pos.X;
            destinationRectangle.Y = (int)pos.Y;

            _renderList.Add(new Renderable
                {
                    Type = RenderableType.Texture,
                    Texture = texture,
                    Color = color,
                    Depth = depth,
                    DestinationRectangle = destinationRectangle,
                    SourceRectangle = sourceRectangle,
                    Origin = origin,
                    Rotation = rotation,
                    Dispose = disposeAfterDraw
                }
            );
        }
        
        #endregion
        
        /// <summary>
        /// Clean the renderer
        /// </summary>
        public void Dispose()
        {
            _renderList?.Clear();
            _renderList = null;
        }
        
        #endregion
    }
}
