using System;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;

namespace WindowShelf {
public class ShelfPalette {
    public string Background="#F3F4F6",Header="#E9ECF0",Text="#303844",Border="#D8DDE4";
    public string Active="#E2E7ED",Hover="#E9EDF2",Secondary="#DCE5F2",SecondaryActive="#C5D6EC";
    public string Accent="#56759A",Close="#66717E",CloseHover="#A53841";
    public static Color Parse(string value,string fallback){try{if(value==null||value.Length!=7||value[0]!='#')throw new FormatException();return Color.FromArgb(255,ColorTranslator.FromHtml(value));}catch{return ColorTranslator.FromHtml(fallback);}}
}
public class AppSettings {
    public int ExpandedWidth=280,IconSize=32,FontSize=16,RowHeight=44,HeaderHeight=34;
    public string FontFamily="Microsoft YaHei UI";
    public int HoverExpandDelayMs=180,HoverCollapseDelayMs=450,RefreshIntervalMs=900,TopInset=0;
    public int ChildIndent=12,CloseButtonWidth=40;
    public bool CenterList=true;
    public ShelfPalette Colors=new ShelfPalette();
    public void Normalize(){
        ExpandedWidth=Clamp(ExpandedWidth,220,800);if(IconSize!=16&&IconSize!=32&&IconSize!=48)IconSize=32;
        FontSize=Clamp(FontSize,10,28);RowHeight=Clamp(RowHeight,Math.Max(IconSize+12,FontSize+16),96);
        HeaderHeight=Clamp(HeaderHeight,28,64);ChildIndent=Clamp(ChildIndent,0,24);CloseButtonWidth=Clamp(CloseButtonWidth,32,64);
        HoverExpandDelayMs=Clamp(HoverExpandDelayMs,0,3000);HoverCollapseDelayMs=Clamp(HoverCollapseDelayMs,100,5000);RefreshIntervalMs=Clamp(RefreshIntervalMs,300,5000);TopInset=Clamp(TopInset,0,500);
        if(String.IsNullOrWhiteSpace(FontFamily))FontFamily="Microsoft YaHei UI";if(Colors==null)Colors=new ShelfPalette();
    }
    static int Clamp(int value,int min,int max){return Math.Max(min,Math.Min(max,value));}
    public static AppSettings Load(string folder,bool writeDefault){
        string path=Path.Combine(folder,"settings.json");AppSettings settings=new AppSettings();
        try{if(File.Exists(path))settings=new JavaScriptSerializer().Deserialize<AppSettings>(File.ReadAllText(path))??settings;
            else if(writeDefault)File.WriteAllText(path,new JavaScriptSerializer().Serialize(settings),new System.Text.UTF8Encoding(false));
        }catch(Exception e){if(writeDefault)try{File.WriteAllText(Path.Combine(folder,"settings-error.txt"),e.GetType().Name+": "+e.Message);}catch{}}
        settings.Normalize();return settings;
    }
}
}
