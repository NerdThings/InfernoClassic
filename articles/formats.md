# File Formats
Here is a list of file formats Inferno has, and specification for them

## Inferno Font
The Inferno Font File Format is an easy way of storing all information about a font including its texture in one compressed file (GZIPPED).

### Specification (V1.2)
The entire file is generated THEN gzipped. This version yields a 25% smaller filesize.

Order of data:
- Header - String - "INFERNOFONT"
- Version - String - "VERSION_1.2"
- Width - Int - Width of font texture
- Height - Int - Height of font texture
- SpaceSize - Int - The number of pixels in a space
- LineHeight - Int - The number of pixels between lines
- ImageBytesCount - Int - Number of bytes in the ImageBytes array
- ImageBytes - Byte[] - Array of UInts (Array of packed color values) converted into a byte array
- CharMapCount - Int - Number of bytes in the CharMap array
- CharMap - Byte[] - Array of Floats (Array of vector components) converted into a byte array

### Specification (V1.1)
The entire file is generated THEN gzipped. This format is outdated.

Order of data:
- Header - String - "INFERNOFONT"
- Version - String - "VERSION_1.1"
- Width - Int - Width of Font Bitmap
- Height - Int - Height of Font Bitmap
- SpaceSize - Int - Amount of pixels a space is
- LineHeight - Int - How many pixels down to move when a new line is met.
- Image - String - CSV of color values in packed formats (Base64 Encoded)
- CharData - String - CSV of floats (Length must be a factor of 4, Base64 Encoded), this is turned into a Vector4 array, with one Vector4 for each character, The X and Y components are the X and Y coordinates of the character on the image. The Z and W components are the width and height of the texture.

### Specification (V1)
The information for this specification is not available, as it was never publicly released. It lacked the ability to store the SpaceSize and LineHeight of the font and was not gzipped.