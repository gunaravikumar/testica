using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Selenium.Scripts.Pages.iConnect
{
    public enum Z3DTools
    {
        [EnumMember(Value = "Interactive Zoom")]
        Interactive_Zoom,

        [EnumMember(Value = "Window Level")]
        Window_Level,

        [EnumMember(Value = "Rotate Tool - Image Center")]
        Rotate_Tool_1_Image_Center,

        [EnumMember(Value = "Rotate Tool - Click Center")]
        Rotate_Tool_1_Click_Center,

        [EnumMember(Value = "Pan")]
        Pan,

        [EnumMember(Value = "Line Measurement")]
        Line_Measurement,

        [EnumMember(Value = "Scrolling Tool")]
        Scrolling_Tool,

        [EnumMember(Value = "Sculpt Tool for 3D - Polygon")]
        Sculpt_Tool_for_3D_1_Polygon,

        [EnumMember(Value = "Sculpt Tool for 3D - Freehand")]
        Sculpt_Tool_for_3D_1_Freehand,

        [EnumMember(Value = "Undo Segmentation")]
        Undo_Segmentation,

        [EnumMember(Value = "Redo Segmentation")]
        Redo_Segmentation,

        [EnumMember(Value = "Selection Tool")]
        Selection_Tool,

        [EnumMember(Value = "Reset")]
        Reset,

        [EnumMember(Value = "Download Image")]
        Download_Image,

        [EnumMember(Value = "Curve Drawing Tool - Manual")]
        Curve_Drawing_Tool_1_Manual,

        [EnumMember(Value = "Curve Drawing Tool - Auto (Vessels)")]
        Curve_Drawing_Tool_1_Auto_2Vessels5,

        [EnumMember(Value = "Curve Drawing Tool - Auto (Colon)")]
        Curve_Drawing_Tool_1_Auto_2Colon5,

        [EnumMember(Value = "Calcium Scoring")]
        Calcium_Scoring,
    }
}
