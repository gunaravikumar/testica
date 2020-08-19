using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium.Scripts.Pages.iConnect
{
    /// <summary>
    /// This enum is created for BluRing Tools
    /// </summary>
    /// 

    public enum BluRingTools
   {
        [EnumMember(Value = "Pan")]
        Pan,

		[EnumMember(Value = "Flip Horizontal")]
		Flip_Horizontal,

		[EnumMember(Value = "Flip Vertical")]
		Flip_Vertical,

		[EnumMember(Value = "Magnifier")]
        Magnifier,

		[EnumMember(Value = "Rotate Clockwise")]
		Rotate_Clockwise,

		[EnumMember(Value = "Rotate Counterclockwise")]
		Rotate_Counterclockwise,

		[EnumMember(Value = "Measure")]
        Line_Measurement,

        [EnumMember(Value = "Cobb Angle")]
        Cobb_Angle,

        [EnumMember(Value = "Free Draw")]
        Free_Draw,

        [EnumMember(Value = "Draw Ellipse")]
        Draw_Ellipse,

        [EnumMember(Value = "Draw Rectangle")]
        Draw_Rectangle,

        [EnumMember(Value = "")]
        Edit_Text,

		[EnumMember(Value = "Angle Measurement")]
		Angle_Measurement,

		[EnumMember(Value = "Window Level")]
        Window_Level,

        [EnumMember(Value = "Calibration Tool")]
        Calibration_Tool,

        [EnumMember(Value = "Invert")]
        Invert,

        [EnumMember(Value = "")]
        Help,

        [EnumMember(Value = "Get Pixel Value")]
        Get_Pixel_Value,

	    [EnumMember(Value = "Delete")]
        Delete_Annotation,

        [EnumMember(Value = "EDIT")]
        Edit_Annotation,

        [EnumMember(Value = "Draw ROI")]
        Draw_ROI,

        [EnumMember(Value = "Reset")]
        Reset,

        [EnumMember(Value = "ViewportTools.SaveAnnotatedSeries.Captions")]
        Save_Series,

        [EnumMember(Value = "ViewportTools.SaveAnnotatedImage.Captions")]
        Save_Annotated_Image,

        [EnumMember(Value = "Add Text")]
        Add_Text,

        [EnumMember(Value = "Interactive Zoom")]
        Interactive_Zoom,

        [EnumMember(Value = "Series Scope")]
        Series_Scope,

        [EnumMember(Value = "Image Scope")]
        Image_Scope,

        [EnumMember(Value = "Remove All Annotations")]
        Remove_All_Annotations,

        [EnumMember(Value = "All in One Tool")]
        All_in_One_Tool,

        [EnumMember(Value = "AutoWL")]
        AutoWL,

        [EnumMember(Value = "Interactive Window Width/Level")]
        Interactive_Window_Width,

        [EnumMember(Value = "Scroll Tool")]
        Scroll_Tool,

        [EnumMember(Value = "Joint Line Measurement")]
        Joint_Line_Measurement,

        [EnumMember(Value = "Transischial Measurement")]
        Transischial_Measurement,

        [EnumMember(Value = "Vertical Plumb Line Measurement")]
        Vertical_Plumb_Line,

        [EnumMember(Value = "Horizontal Plumb Line Measurement")]
        Horizontal_Plumb_Line
    }    
}
