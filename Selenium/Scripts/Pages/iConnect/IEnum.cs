using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Selenium.Scripts.Pages.iConnect
{
    public abstract class IEnum
    {
        public enum ViewerTools
        {
            AllinOneTool, ScrollTool, WindowLevel, AutoWindowLevel, Invert, EdgeEnhancementInteractive,
            EdgeEnhancementMedium3x3, EdgeEnhancementLow5x5, EdgeEnhancementMedium11x11,
            Zoom, Magnifierx2, Magnifierx3, Magnifierx4, Pan, LineMeasurement, CalibrationTool,
            TransischialMeasurement, JointLineMeasurement, HorizontalPlumbLine, VerticalPlumbLine,
            CobbAngle, AngleMeasurement, AddText, EditText, FreeDraw,
            DrawEllipse, DrawRectangle, DrawROI, GetPixelValue, EditAnnotations,
            DeleteAnnotation, RemoveAllAnnotations, Cine, GlobalStack, ToggleText, LocalizerLine,
            LinkAll, LinkSelected, LinkSelectedOffset, LinkAllOffset, Unlink,
            FlipHorizontal, FlipVertical, RotateClockwise, RotateCounterclockwise,
            PrintView, SaveSeries, SaveAnnotatedImages, Reset, FullScreen,
            SeriesViewer1x1, SeriesViewer1x2, SeriesViewer1x3, SeriesViewer2x2, SeriesViewer2x3,
            UserPreference, DownloadDocument, NextSeries, PreviousSeries, SeriesScope, ImageScope,
            ImageLayout2x2, ImageLayout4x4, ImageLayout3x3, ImageLayout2x1, ImageLayout1x2, ImageLayout1x1,
            Help, Close, EmailStudy, AddToConferenceFolder, TransferStudy, GrantAccesstoStudy, GeneratePDFReport,ArchiveStudy,
            NominateforArchive,ExamMode,InteractiveZoom
        }

        public enum Locale
        {
            en_US,
            zh_CN,
            es_ES,
            ar_SA
        }

       
    }
}
