using System;
using System.Drawing;
using System.Drawing.Imaging;

class SteganographyHelper {
  public enum State {
    Hiding,
    Filling_With_Zeros
  };

  public static Bitmap embedText(string text, Bitmap bmp) {

    State state = State.Hiding;
    int charIndex = 0;
    int charValue = 0;
    long pixelElementIndex = 0;
    int zeros = 0;
    int R = 0, G = 0, B = 0;

    for (int i = 0; i < bmp.Height; i++) {
      for (int j = 0; j < bmp.Width; j++) {
        Color pixel = bmp.GetPixel(j, i);
        R = pixel.R - pixel.R % 2;
        G = pixel.G - pixel.G % 2;
        B = pixel.B - pixel.B % 2;

        for (int n = 0; n < 3; n++) {
          if (pixelElementIndex % 8 == 0) {
            if (state == State.Filling_With_Zeros && zeros == 8) {
              if ((pixelElementIndex - 1) % 3 < 2) {
                bmp.SetPixel(j, i, Color.FromArgb(R, G, B));
              }
              return bmp;
            }

            if (charIndex >= text.Length) {
              state = State.Filling_With_Zeros;
            } else {
              charValue = text[charIndex++];
            }
          }

          switch (pixelElementIndex % 3) {
          case 0: {
            if (state == State.Hiding) {
              R += charValue % 2;
              charValue /= 2;
            }
          }
          break;
          case 1: {
            if (state == State.Hiding) {
              G += charValue % 2;

              charValue /= 2;
            }
          }
          break;
          case 2: {
            if (state == State.Hiding) {
              B += charValue % 2;

              charValue /= 2;
            }

            bmp.SetPixel(j, i, Color.FromArgb(R, G, B));
          }
          break;
          }

          pixelElementIndex++;

          if (state == State.Filling_With_Zeros) {
            zeros++;
          }
        }
      }
    }

    return bmp;
  }

  public static void Main() {

    Bitmap source = (Bitmap) Image.FromFile("Stego.bmp");
    Console.WriteLine("Enter a phrase to be hidden in plain sight");
    string phrase = Console.ReadLine();
    Bitmap b = new Bitmap(embedText(phrase, source));
    b.Save("Result.bmp", ImageFormat.Bmp);
  }
}
