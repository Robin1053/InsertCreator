using HgSoftware.InsertCreator.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using HgSoftware.InsertCreator.Extensions;

namespace HgSoftware.InsertCreator.Model
{
    public class FadeInWriter
    {
        #region Private Fields

        private readonly PositionData _positionData;
        private readonly CorporateDesignPositionData _corporateDesignPositionData;
        private readonly BiblewordPositionData _biblewordPositionData;

        #endregion Private Fields

        #region Public Constructors

        public FadeInWriter(PositionData positionData, CorporateDesignPositionData corporateDesignPositionData, BiblewordPositionData biblewordPositionData)
        {
            _positionData = positionData;
            _corporateDesignPositionData = corporateDesignPositionData;
            _biblewordPositionData = biblewordPositionData;
            CurrentFade = LoadFrame(!Properties.Settings.Default.UseGreenscreen, Properties.Settings.Default.LogoAsCornerlogo);
        }

        #endregion Public Constructors

        #region Public Events

        public event EventHandler<Bitmap> OnInsertUpdate;

        #endregion Public Events

        #region Public Properties

        public Bitmap CurrentFade { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public void LoadImages()
        {
            Bitmap image = LoadFrame(!Properties.Settings.Default.UseGreenscreen, Properties.Settings.Default.LogoAsCornerlogo);
            var drawingTool = Graphics.FromImage(image);
            DrawLogo(drawingTool, _positionData);
            image.Save($"{Environment.GetEnvironmentVariable("userprofile")}/InsertCreator/Insert.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        public void ResetFade()
        {
            Bitmap image = LoadFrame(!Properties.Settings.Default.UseGreenscreen, Properties.Settings.Default.LogoAsCornerlogo);
            image.Save($"{Environment.GetEnvironmentVariable("userprofile")}/InsertCreator/Insert.png", System.Drawing.Imaging.ImageFormat.Png);
            CurrentFade = image;
            OnInsertUpdate?.Invoke(this, image);
        }

     
        public void WriteFade(IInsertData insert)
        {
            Bitmap image = SelectFadeWriter(insert);
            if (image != null)
            {
                CreateInsert(image);
            }
        }

        public void SaveFade(IInsertData insert)
        {
            Bitmap image = SelectFadeWriter(insert);
            if (image != null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "png files (*.png)|*.png";
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.InitialDirectory = Environment.GetEnvironmentVariable("userprofile");
                saveFileDialog1.FileName = $"{insert.FirstLine}_{insert.SecondLine}";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    image.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private Bitmap SelectFadeWriter(IInsertData insert)
        {
            var greenScreen = !Properties.Settings.Default.UseGreenscreen;
            var cornerbug = Properties.Settings.Default.LogoAsCornerlogo;

            switch (insert)
            {
                case CustomInsert _:
                    return WriteCustom(insert as CustomInsert, greenScreen, cornerbug);

                case HymnalData _:
                    return WriteHymnalFade(insert as HymnalData, greenScreen, cornerbug);

                case MinistryGridViewModel _:
                    return WriteMinistryFade(insert as MinistryGridViewModel, greenScreen, cornerbug);

                case BibleData _:
                    return WriteBibleFade(insert as BibleData, greenScreen, cornerbug);
            }

            return null;
        }

        private void CreateInsert(Bitmap image)
        {
            image.Save($"{Environment.GetEnvironmentVariable("userprofile")}/InsertCreator/Insert.png", System.Drawing.Imaging.ImageFormat.Png);
            CurrentFade = image;
            OnInsertUpdate?.Invoke(this, image);
        }

        private Bitmap CreateCustomInsertDouble(string textLaneOne, string textLaneTwo, bool transparent = true, bool useCornerBug = false)
        {
            if (Properties.Settings.Default.UseCorporateDesign2026)
                return CreateCorporateDesignInsert(textLaneOne, textLaneTwo, transparent, useCornerBug);

            Bitmap image = LoadFrame(transparent, useCornerBug);
            var drawingTool = Graphics.FromImage(image);
            DrawRectangle(drawingTool, _positionData);

            drawingTool.DrawString(
            textLaneOne,
            _positionData.FontTextTwoRowFirstLine,
            new SolidBrush(Color.Black), _positionData.TextTwoRowFirstLinePosition);

            drawingTool.DrawString(
             textLaneTwo,
             _positionData.FontTextTwoRowSecondLine,
             new SolidBrush(Color.Black), _positionData.TextTwoRowSecondLinePosition);

            DrawLogo(drawingTool, _positionData);

            return image;
        }

        private Bitmap CreateCustomInsertSingle(string text, bool transparent = true, bool useCornerBug = false)
        {
            if (Properties.Settings.Default.UseCorporateDesign2026)
                return CreateCorporateDesignInsert(text, string.Empty, transparent, useCornerBug);

            Bitmap image = LoadFrame(transparent, useCornerBug);
            var drawingTool = Graphics.FromImage(image);
            DrawRectangle(drawingTool, _positionData);

            drawingTool.DrawString(
            text,
            _positionData.FontTextOneRowFirstLine,
            new SolidBrush(Color.Black), _positionData.TextOneRowFirstLinePosition);

            DrawLogo(drawingTool, _positionData);

            return image;
        }

        private Bitmap CreateBibleInsert(BibleData bibleData, bool transparent = true, bool useCornerBug = false)
        {
            if (Properties.Settings.Default.UseCorporateDesign2026)
                return CreateCorporateDesignInsert("Bibelwort", $"{bibleData.BibleBook} {bibleData.BibleChapter}, {bibleData.BibleVerse}", transparent, useCornerBug);

            Bitmap image = LoadFrame(transparent, useCornerBug);
            var drawingTool = Graphics.FromImage(image);
            DrawRectangle(drawingTool, _positionData);

            drawingTool.DrawString(
             "Bibelwort",
            _positionData.FontTextTwoRowFirstLine,
             new SolidBrush(Color.Black), _positionData.TextTwoRowFirstLinePosition);

            drawingTool.DrawString(
             $"{bibleData.BibleBook} {bibleData.BibleChapter}, {bibleData.BibleVerse}",
               _positionData.FontTextTwoRowSecondLine,
               new SolidBrush(Color.Black), _positionData.TextTwoRowSecondLinePosition);

            DrawLogo(drawingTool, _positionData);

            return image;
        }

     
        private Bitmap CreateHymnalInsertPicture(HymnalData hymnalData, bool transparent = true, bool useCornerBug = false)
        {
            if (Properties.Settings.Default.UseCorporateDesign2026)
                return CreateCorporateDesignInsert($"{hymnalData.Book} {hymnalData.Number}{hymnalData.SongVerses}", hymnalData.Name, transparent, useCornerBug);

            Bitmap image = LoadFrame(transparent, useCornerBug);

            var drawingTool = Graphics.FromImage(image);

            DrawRectangle(drawingTool, _positionData);

            drawingTool.DrawString(
             $"{hymnalData.Book} {hymnalData.Number}{hymnalData.SongVerses}",
             _positionData.FontTextTwoRowFirstLine,
             new SolidBrush(Color.Black), _positionData.TextTwoRowFirstLinePosition);

            drawingTool.DrawString(
                hymnalData.Name,
                _positionData.FontTextTwoRowSecondLine,
                new SolidBrush(Color.Black), _positionData.TextTwoRowSecondLinePosition);

            DrawLogo(drawingTool, _positionData);

            return image;
        }

        private Bitmap CreateHymnalInsertPictureMeta(HymnalData hymnalData, bool transparent, bool useCornerBug)
        {
            if (Properties.Settings.Default.UseCorporateDesign2026)
                return CreateCorporateDesignInsert($"{hymnalData.Book} {hymnalData.Number}{hymnalData.SongVerses}", hymnalData.Name, transparent, useCornerBug);

            Bitmap image = LoadFrame(transparent, useCornerBug);

            var drawingTool = Graphics.FromImage(image);

            DrawRectangle(drawingTool, _positionData);

            drawingTool.DrawString(
             $"{hymnalData.Book} {hymnalData.Number}{hymnalData.SongVerses}",
             _positionData.FontTextFourRowFirstLine,
             new SolidBrush(Color.Black), _positionData.TextFourRowFirstLinePosition);

            drawingTool.DrawString(
                hymnalData.Name,
                _positionData.FontTextFourRowSecondLine,
                new SolidBrush(Color.Black), _positionData.TextFourRowSecondLinePosition);

            drawingTool.DrawString(
               hymnalData.TextAutor,
               _positionData.FontTextFourRowThirdLine,
               new SolidBrush(Color.Black), _positionData.TextFourRowThirdLinePosition);

            drawingTool.DrawString(
               hymnalData.MelodieAutor,
               _positionData.FontTextFourRowFourthLine,
               new SolidBrush(Color.Black), _positionData.TextFourRowFourthLinePosition);

            DrawLogo(drawingTool, _positionData);

            return image;
        }

        private Bitmap CreateMinistrieInsert(MinistryGridViewModel ministry, bool transparent = true, bool useCornerBug = false)
        {
            if (Properties.Settings.Default.UseCorporateDesign2026)
                return CreateCorporateDesignInsert($"{ministry.ForeName} {ministry.SureName}", ministry.Function, transparent, useCornerBug);

            Bitmap image = LoadFrame(transparent, useCornerBug);

            var drawingTool = Graphics.FromImage(image);
            DrawRectangle(drawingTool, _positionData);

            drawingTool.DrawString(
             $"{ministry.ForeName} {ministry.SureName}",
             _positionData.FontTextTwoRowFirstLine,
             new SolidBrush(Color.Black), _positionData.TextTwoRowFirstLinePosition);

            drawingTool.DrawString(
             ministry.Function,
             _positionData.FontTextTwoRowSecondLine,
             new SolidBrush(Color.Black), _positionData.TextTwoRowSecondLinePosition);

            DrawLogo(drawingTool, _positionData);
            return image;
        }

        private Bitmap CreateCorporateDesignInsert(string firstLine, string secondLine, bool transparent = true, bool useCornerBug = false)
        {
            Bitmap image = LoadFrame(transparent, useCornerBug);
            var drawingTool = Graphics.FromImage(image);

            DrawCorporateBackground(drawingTool);
            DrawCorporateWhiteStripe(drawingTool);
            DrawCorporateText(drawingTool, firstLine, secondLine);

            return image;
        }

        private void DrawCorporateBackground(Graphics drawingTool)
        {
            using (var brush = new SolidBrush(_corporateDesignPositionData.BarColor))
            {
                drawingTool.FillRectangle(brush, new Rectangle(_corporateDesignPositionData.FirstBarPosition, _corporateDesignPositionData.FirstBarSize));
                drawingTool.FillRectangle(brush, new Rectangle(_corporateDesignPositionData.SecondBarPosition, _corporateDesignPositionData.SecondBarSize));
            }
        }

        private void DrawCorporateText(Graphics drawingTool, string firstLine, string secondLine)
        {
            using (var firstBrush = new SolidBrush(_corporateDesignPositionData.TextColor))
            using (var secondBrush = new SolidBrush(_corporateDesignPositionData.TextColor))
            using (var firstFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap })
            using (var secondFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap })
            {
                var firstBounds = new RectangleF(_corporateDesignPositionData.FirstTextPosition.X, _corporateDesignPositionData.FirstBarPosition.Y, _corporateDesignPositionData.FirstBarSize.Width - (_corporateDesignPositionData.FirstTextPosition.X - _corporateDesignPositionData.FirstBarPosition.X), _corporateDesignPositionData.FirstBarSize.Height);
                var secondBounds = new RectangleF(_corporateDesignPositionData.SecondTextPosition.X, _corporateDesignPositionData.SecondBarPosition.Y, _corporateDesignPositionData.SecondBarSize.Width - (_corporateDesignPositionData.SecondTextPosition.X - _corporateDesignPositionData.SecondBarPosition.X), _corporateDesignPositionData.SecondBarSize.Height);

                drawingTool.DrawString(firstLine ?? string.Empty, _corporateDesignPositionData.FirstTextFont, firstBrush, firstBounds, firstFormat);
                drawingTool.DrawString(secondLine ?? string.Empty, _corporateDesignPositionData.SecondTextFont, secondBrush, secondBounds, secondFormat);
            }
        }

        private void DrawCorporateWhiteStripe(Graphics drawingTool)
        {
            using (var brush = new SolidBrush(_corporateDesignPositionData.StripeColor))
            {
                drawingTool.FillRectangle(brush, new Rectangle(_corporateDesignPositionData.WhiteStripePosition.X, _corporateDesignPositionData.FirstBarPosition.Y, _corporateDesignPositionData.WhiteStripeWidth, (_corporateDesignPositionData.SecondBarPosition.Y + _corporateDesignPositionData.SecondBarSize.Height) - _corporateDesignPositionData.FirstBarPosition.Y));
            }
        }

        private void DrawLogo(Graphics drawingTool, IPositionData positionData)
        {
            if (File.Exists($"{Environment.GetEnvironmentVariable("userprofile")}/InsertCreator/Logo.png"))
            {
                var image = new Bitmap($"{Environment.GetEnvironmentVariable("userprofile")}/InsertCreator/Logo.png");
                drawingTool.DrawImage(image, new Rectangle(Point.Round(positionData.LogoPosition), new Size(positionData.SizeLogo, positionData.SizeLogo)));
            }
        }

        private void DrawRectangle(Graphics drawingTool, IPositionData positionData)
        {
            System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(positionData.TransparencyRectangle, 255, 255, 255));

            drawingTool.FillRectangle(myBrush, new Rectangle(positionData.RectanglePosition.X, positionData.RectanglePosition.Y, positionData.SizeRectangle.Width, positionData.SizeRectangle.Height));
            myBrush.Dispose();
        }

        private Bitmap LoadFrame(bool transparent, bool useCornerBug)
        {
            var transparentFrame = $"{Directory.GetCurrentDirectory()}/DataSource/InsertFrameTrans.png";
            var greenFrame = $"{Directory.GetCurrentDirectory()}/DataSource/InsertFrameGreen.png";
            Bitmap image;

            if (transparent)
                image = new Bitmap(transparentFrame);
            else
                image = new Bitmap(greenFrame);

            if (useCornerBug && File.Exists($"{Environment.GetEnvironmentVariable("userprofile")}/InsertCreator/Logo.png"))
            {
                var drawingTool = Graphics.FromImage(image);
                var logoImage = new Bitmap($"{Environment.GetEnvironmentVariable("userprofile")}/InsertCreator/Logo.png");
                var cornerbugPosition = Properties.Settings.Default.UseCorporateDesign2026 ? _corporateDesignPositionData.CornerbugPosition : _positionData.CornerbugPosition;
                var cornerbugSize = Properties.Settings.Default.UseCorporateDesign2026 ? _corporateDesignPositionData.SizeCornerbug : _positionData.SizeCornerbug;

                drawingTool.DrawImage(logoImage, new Rectangle(Point.Round(cornerbugPosition), new Size(cornerbugSize, cornerbugSize)));
            }
            return image;
        }

        private Bitmap WriteCustom(CustomInsert insert, bool greenScreen, bool cornerbug)
        {
            if (String.IsNullOrEmpty(insert.LineOne))
            {
                return CreateCustomInsertSingle(insert.LineTwo, greenScreen, cornerbug);
            }

            if (String.IsNullOrEmpty(insert.LineTwo))
            {
                return CreateCustomInsertSingle(insert.LineOne, greenScreen, cornerbug);
            }
            return CreateCustomInsertDouble(insert.LineOne, insert.LineTwo, greenScreen, cornerbug);
        }

        private Bitmap WriteHymnalFade(HymnalData hymnalData, bool greenScreen, bool cornerbug)
        {
            if (Properties.Settings.Default.ShowComponistAndAutor)
                return CreateHymnalInsertPictureMeta(hymnalData, greenScreen, cornerbug);
            else
                return CreateHymnalInsertPicture(hymnalData, greenScreen, cornerbug);
        }

        private Bitmap WriteMinistryFade(MinistryGridViewModel ministry, bool greenScreen, bool cornerbug)
        {
            return CreateMinistrieInsert(ministry, greenScreen, cornerbug);
        }

        private Bitmap WriteBibleFade(BibleData bibleData, bool greenScreen, bool cornerbug)
        {
            if (string.IsNullOrEmpty(bibleData.BibleText))
                return CreateBibleInsert(bibleData, greenScreen, cornerbug);
            return CreateFullscreenBibleInsert(bibleData, greenScreen);
        }

        private Bitmap CreateFullscreenBibleInsert(BibleData bibleData, bool transparent)
        {
            if (Properties.Settings.Default.UseCorporateDesign2026)
                return CreateCorporateDesignFullscreenBibleInsert(bibleData, transparent);

            Bitmap image = LoadFrame(transparent, false);
            var drawingTool = Graphics.FromImage(image);
            DrawRectangle(drawingTool, _biblewordPositionData);
            DrawLogo(drawingTool, _biblewordPositionData);

            drawingTool.DrawString(
           "Bibelwort",
           _biblewordPositionData.FontTextHeadline,
           new SolidBrush(Color.Black), _biblewordPositionData.HeadlineTextFirstLine);

            drawingTool.DrawString(
          $"{bibleData.BibleBook} {bibleData.BibleChapter}, {bibleData.BibleVerse}",
          _biblewordPositionData.FontTextHeadline,
           new SolidBrush(Color.Black), _biblewordPositionData.HeadlineTextSecondLine);

            DrawBibleText(drawingTool, bibleData.BibleText);

            return image;
        }

        private Bitmap CreateCorporateDesignFullscreenBibleInsert(BibleData bibleData, bool transparent)
        {
            Bitmap image = LoadFrame(transparent, false);
            var drawingTool = Graphics.FromImage(image);

            var boxRectangle = new Rectangle(_biblewordPositionData.RectanglePosition, _biblewordPositionData.SizeRectangle);
            using (var backgroundBrush = new SolidBrush(Color.FromArgb(_biblewordPositionData.TransparencyRectangle, _corporateDesignPositionData.BarColor)))
            using (var stripeBrush = new SolidBrush(_corporateDesignPositionData.StripeColor))
            using (var textBrush = new SolidBrush(_corporateDesignPositionData.TextColor))
            {
                drawingTool.FillRectangle(backgroundBrush, boxRectangle);
                drawingTool.FillRectangle(stripeBrush, new Rectangle(boxRectangle.X, boxRectangle.Y, _corporateDesignPositionData.WhiteStripeWidth, boxRectangle.Height));

                drawingTool.DrawString(
                   "Bibelwort",
                   _biblewordPositionData.FontTextHeadline,
                   textBrush, _biblewordPositionData.HeadlineTextFirstLine);

                drawingTool.DrawString(
                  $"{bibleData.BibleBook} {bibleData.BibleChapter}, {bibleData.BibleVerse}",
                  _biblewordPositionData.FontTextHeadline,
                   textBrush, _biblewordPositionData.HeadlineTextSecondLine);
            }

            DrawBibleText(drawingTool, bibleData.BibleText, _corporateDesignPositionData.TextColor);

            return image;
        }

        private void DrawBibleText(Graphics drawingTool, string bibleText, Color? textColor = null)
        {
            var bibleTextWithoutLinks = Regex.Replace(bibleText, "[ ][(].+[)]", string.Empty);

            var verses = Regex.Split(bibleTextWithoutLinks.TrimStart(' ').TrimStart(Convert.ToChar(160)), "([0-9]+.[^0-9]+)").Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
            var color = textColor ?? Color.Black;

            var count = 0;

            using (var textBrush = new SolidBrush(color))
            {
                foreach (var item in verses)
                {
                    var verse = Regex.Match(item, "[0-9]+").Value;
                    var text = item.Replace($"{verse}", "").Trim(' ').Trim(Convert.ToChar(160)).Replace(Convert.ToChar(160), ' ');

                    if (count == 8)
                        return;

                    drawingTool.DrawString(verse, _biblewordPositionData.FontTextBody, textBrush, _biblewordPositionData.Versenumbers[count]);

                    var lines = text.JustifyParagraph(_biblewordPositionData.FontTextBody, _biblewordPositionData.MaxTextLength).Split(new string[] { "\r\n" }, StringSplitOptions.None);

                    foreach (var line in lines)
                    {
                        if (count == 8)
                            return;
                        drawingTool.DrawString(line, _biblewordPositionData.FontTextBody, textBrush, _biblewordPositionData.TextLines[count]);
                        count++;
                    }
                }
            }
        }

        #endregion Private Methods
    }
}