using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inferno.Runtime.Core
{
    public sealed class Camera
    {
        #region Properties

        /// <summary>
        /// The Camera Position
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// The Camera Zoom
        /// </summary>
        public float Zoom { get; set; }

        /// <summary>
        /// The Camera Rotation
        /// </summary>
        public float Rotation { get; private set; }

        /// <summary>
        /// Viewport Width
        /// </summary>
        public int ViewportWidth { get; set; }

        /// <summary>
        /// Viewport Height
        /// </summary>
        public int ViewportHeight { get; set; }

        /// <summary>
        /// Viewport Center
        /// </summary>
        public Vector2 ViewportCenter
        {
            get
            {
                return new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);
            }
        }
        /// <summary>
        /// Viewport Center
        /// </summary>
        [System.Obsolete("Access to the window size is not required, this will be removed in a future release.")]
        public Vector2 ViewCenter
        {
            get
            {
                return new Vector2(ParentGame.WindowWidth * 0.5f, ParentGame.WindowHeight * 0.5f);
            }
        }
        /// <summary>
        /// Translation Martrix
        /// </summary>
        public Matrix TranslationMatrix
        {
            get
            {
                /*return Matrix.CreateTranslation(-(int)Position.X,
                   -(int)Position.Y, 0) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(((float)ParentGame.WindowWidth / (float)ParentGame.VirtualWidth) * Zoom, ((float)ParentGame.WindowHeight / (float)ParentGame.VirtualHeight) * Zoom, 1) *
                   Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));*/

                return Matrix.CreateTranslation(-(int)Position.X,
                   -(int)Position.Y, 0) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom, Zoom, 1) *
                   Matrix.CreateTranslation(new Vector3(ViewportCenter, 0));
            }
        }
        /// <summary>
        /// Viewport boundary
        /// </summary>
        /// <returns>Viewport boundarys</returns>
        public Rectangle ViewportWorldBoundry
        {
            get
            {
                Vector2 viewPortCorner = ScreenToWorld(new Vector2(0, 0));
                Vector2 viewPortBottomCorner =
                   ScreenToWorld(new Vector2(ViewportWidth, ViewportHeight));

                return new Rectangle((int)viewPortCorner.X,
                   (int)viewPortCorner.Y,
                   (int)(viewPortBottomCorner.X - viewPortCorner.X),
                   (int)(viewPortBottomCorner.Y - viewPortCorner.Y));
            }
        }

        private Game ParentGame;
        private State ParentState;
        #endregion

        #region Constructors

        /// <summary>
        /// Create a Camera
        /// </summary>
        public Camera(Game parentgame, State parentState) : this(parentgame, parentState, 1.0f) { }

        /// <summary>
        /// Create a Camara
        /// </summary>
        /// <param name="zoom">Zoom</param>
        public Camera(Game parentgame, State parentState, float zoom)
        {
            ParentGame = parentgame;
            ParentState = parentState;

            this.ViewportHeight = parentgame.WindowHeight;
            this.ViewportWidth = parentgame.WindowWidth;

            this.Position = new Vector2(ViewportWidth / 2, ViewportHeight / 2);

            if (zoom > 0.25f)
            {
                Zoom = zoom;
            }
            else
            {
                Zoom = 1.0f;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Center the camera on a position
        /// </summary>
        /// <param name="position">Center Location</param>
        public void CenterOn(Vector2 position)
        {
            /*if (position.X < ViewportWorldBoundry.Width / 2)
            {
                Position = new Vector2(ViewportWorldBoundry.Width / 2, Position.Y);
            }
            else if (position.X > ParentState.Width - ViewportWorldBoundry.Width / 2)
            {
                Position = new Vector2(ParentState.Width - ViewportWorldBoundry.Width / 2, Position.Y);
            }
            else
            {
                Position = new Vector2(position.X, Position.Y);
            }

            if (position.Y < ViewportWorldBoundry.Height / 2)
            {
                Position = new Vector2(Position.X, ViewportWorldBoundry.Height / 2);
            }
            else if (position.Y > ParentState.Height - ViewportWorldBoundry.Height / 2)
            {
                Position = new Vector2(Position.X, ParentState.Height - ViewportWorldBoundry.Height / 2);
            }
            else
            {
                Position = new Vector2(Position.X, position.Y);
            }*/
            Position = new Vector2(Position.X, position.Y);
        }

        /// <summary>
        /// Convert World Coorinate to Screen Cooridnate
        /// </summary>
        /// <param name="worldPosition">World Position</param>
        /// <returns>Screen Position</returns>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TranslationMatrix);
        }

        /// <summary>
        /// Convert Screen Coorinate to World Cooridnate
        /// </summary>
        /// <param name="screenPosition">Screen Position</param>
        /// <returns>World Position</returns>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(TranslationMatrix));
        }

        /// <summary>
        /// Determine if it is in range
        /// </summary>
        /// <param name="bounds">Range to check</param>
        /// <returns></returns>
        public bool Drawable(Rectangle bounds)
        {
            if (bounds.X + bounds.Width < Position.X - ViewportWidth / 2 && bounds.X < Position.X - ViewportWidth / 2)
            {
                return false;
            }

            if (bounds.X + bounds.Width > Position.X + ViewportWidth / 2 && bounds.X > Position.X + ViewportWidth / 2)
            {
                return false;
            }

            if (bounds.Y + bounds.Height < Position.Y - ViewportHeight / 2 && bounds.Y < Position.Y - ViewportHeight / 2)
            {
                return false;
            }

            if (bounds.Y + bounds.Height > Position.Y + ViewportHeight / 2 && bounds.Y > Position.Y + ViewportHeight / 2)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
