using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace WindowShelf {
static class Native {
    public delegate bool EnumProc(IntPtr h, IntPtr l);
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L,T,R,B; public Rectangle Rect { get { return Rectangle.FromLTRB(L,T,R,B); } } }
    [DllImport("user32.dll")] public static extern bool EnumWindows(EnumProc p, IntPtr l);
    [DllImport("user32.dll")] public static extern bool EnumChildWindows(IntPtr h, EnumProc p, IntPtr l);
    [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern IntPtr FindWindow(string cls,string title);
    [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern IntPtr FindWindowEx(IntPtr p,IntPtr after,string cls,string title);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h,out RECT r);
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr h);
    [DllImport("user32.dll")] public static extern bool IsWindow(IntPtr h);
    [DllImport("user32.dll")] public static extern bool IsIconic(IntPtr h);
    [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] public static extern IntPtr GetWindow(IntPtr h,uint cmd);
    [DllImport("user32.dll")] public static extern IntPtr GetAncestor(IntPtr h,uint flag);
    [DllImport("user32.dll")] public static extern IntPtr GetLastActivePopup(IntPtr h);
    [DllImport("user32.dll",EntryPoint="GetWindowLongPtrW")] public static extern IntPtr GetWindowLongPtr(IntPtr h,int n);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)] public static extern int GetWindowText(IntPtr h,StringBuilder s,int n);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)] public static extern int GetClassName(IntPtr h,StringBuilder s,int n);
    [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h,out uint pid);
    [DllImport("user32.dll",SetLastError=true)] public static extern bool PostMessage(IntPtr h,uint msg,IntPtr w,IntPtr l);
    [DllImport("user32.dll")] public static extern bool ShowWindowAsync(IntPtr h,int cmd);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr h,IntPtr after,int x,int y,int w,int height,uint flags);
    [DllImport("user32.dll")] public static extern bool RegisterHotKey(IntPtr h,int id,uint modifiers,uint key);
    [DllImport("user32.dll")] public static extern bool UnregisterHotKey(IntPtr h,int id);
    [DllImport("user32.dll")] public static extern bool SetProcessDpiAwarenessContext(IntPtr ctx);
    [DllImport("user32.dll")] public static extern uint GetDpiForWindow(IntPtr h);
    [DllImport("dwmapi.dll")] public static extern int DwmGetWindowAttribute(IntPtr h,int attr,out int value,int size);
    [DllImport("kernel32.dll")] public static extern IntPtr OpenProcess(uint access,bool inherit,uint pid);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] public static extern bool QueryFullProcessImageName(IntPtr h,int flags,StringBuilder name,ref int size);
    [DllImport("kernel32.dll")] public static extern bool CloseHandle(IntPtr h);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode)] public static extern int GetApplicationUserModelId(IntPtr h,ref uint size,StringBuilder name);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)] public static extern IntPtr SendMessageTimeout(IntPtr h,uint msg,IntPtr w,IntPtr l,uint flags,uint timeout,out IntPtr result);
    [DllImport("user32.dll",EntryPoint="GetClassLongPtrW")] public static extern IntPtr GetClassLongPtr(IntPtr h,int n);
    [DllImport("shell32.dll",CharSet=CharSet.Unicode)] public static extern int SHDefExtractIcon(string path,int index,uint flags,out IntPtr large,out IntPtr small,uint size);
    [DllImport("user32.dll")] public static extern bool DestroyIcon(IntPtr icon);
    [StructLayout(LayoutKind.Sequential)] public struct POINT {public int X,Y;}
    [StructLayout(LayoutKind.Sequential)] public struct WINDOWPLACEMENT {public int Length;public uint Flags,ShowCmd;public POINT Min,Max;public RECT Normal;}
    [DllImport("user32.dll",SetLastError=true)] public static extern bool GetWindowPlacement(IntPtr h,ref WINDOWPLACEMENT p);
    [DllImport("user32.dll",SetLastError=true)] public static extern bool SetWindowPlacement(IntPtr h,ref WINDOWPLACEMENT p);
    [StructLayout(LayoutKind.Sequential)] public struct MONITORINFO {public int Size;public RECT Monitor,Work;public uint Flags;}
    [DllImport("user32.dll")] public static extern IntPtr MonitorFromWindow(IntPtr h,uint flags);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)] public static extern bool GetMonitorInfo(IntPtr monitor,ref MONITORINFO info);
    public static bool OnSecondaryMonitor(IntPtr h){var monitor=MonitorFromWindow(h,2);var info=new MONITORINFO{Size=Marshal.SizeOf(typeof(MONITORINFO))};return monitor!=IntPtr.Zero&&GetMonitorInfo(monitor,ref info)&&(info.Flags&1)==0;}
    public static string Title(IntPtr h) { var s=new StringBuilder(2048); GetWindowText(h,s,s.Capacity);return s.ToString(); }
    public static string Class(IntPtr h) { var s=new StringBuilder(256);GetClassName(h,s,s.Capacity);return s.ToString(); }
    public static bool Cloaked(IntPtr h) {int v;return DwmGetWindowAttribute(h,14,out v,4)==0 && v!=0;}
    public static int Pid(IntPtr h){uint p;GetWindowThreadProcessId(h,out p);return (int)p;}
    public static string PathOf(int pid) {var h=OpenProcess(0x1000,false,(uint)pid);if(h==IntPtr.Zero)return "";try{int n=2048;var s=new StringBuilder(n);return QueryFullProcessImageName(h,0,s,ref n)?s.ToString():"";}finally{CloseHandle(h);}}
    public static string AppId(int pid) {var h=OpenProcess(0x1000,false,(uint)pid);if(h==IntPtr.Zero)return "";try{uint n=512;var s=new StringBuilder((int)n);return GetApplicationUserModelId(h,ref n,s)==0?s.ToString():"";}catch{return "";}finally{CloseHandle(h);}}
    public static Bitmap WindowIcon(IntPtr h,string path,int size) {
        Bitmap fallback=null;
        try {IntPtr icon;SendMessageTimeout(h,0x7f,new IntPtr(1),IntPtr.Zero,2,60,out icon);
            if(icon==IntPtr.Zero)icon=GetClassLongPtr(h,-14);
            if(icon!=IntPtr.Zero){using(var clone=(Icon)Icon.FromHandle(icon).Clone())fallback=clone.ToBitmap();if(fallback.Width==size&&fallback.Height==size)return fallback;}
            if(File.Exists(path)){
                IntPtr large=IntPtr.Zero,small=IntPtr.Zero;
                try{if(SHDefExtractIcon(path,0,0,out large,out small,(uint)size)==0&&large!=IntPtr.Zero){using(var copy=(Icon)Icon.FromHandle(large).Clone()){var result=copy.ToBitmap();if(fallback!=null)fallback.Dispose();return result;}}}
                finally{if(large!=IntPtr.Zero)DestroyIcon(large);if(small!=IntPtr.Zero)DestroyIcon(small);}
            }
            if(fallback==null){SendMessageTimeout(h,0x7f,IntPtr.Zero,IntPtr.Zero,2,60,out icon);if(icon!=IntPtr.Zero){using(var clone=(Icon)Icon.FromHandle(icon).Clone())fallback=clone.ToBitmap();}}
        }catch{}return fallback??SystemIcons.Application.ToBitmap();
    }
}
class WindowItem {
    public IntPtr Handle; public int Pid; public string Id,Group,App,Title,Alias,Path; public Bitmap Icon;public bool Secondary;
    public string Label {get{return String.IsNullOrWhiteSpace(Alias)?Title:Alias;}}
}
static class WindowMover {
    public static Rectangle MapNormalRect(Rectangle rect,Screen source,Screen target){
        // WINDOWPLACEMENT uses workspace coordinates for ordinary top-level windows.
        int x=rect.X+(source.WorkingArea.Left-source.Bounds.Left)-source.WorkingArea.Left;
        int y=rect.Y+(source.WorkingArea.Top-source.Bounds.Top)-source.WorkingArea.Top;
        int width=Math.Min(Math.Max(80,rect.Width),target.WorkingArea.Width),height=Math.Min(Math.Max(60,rect.Height),target.WorkingArea.Height);
        int screenX=target.WorkingArea.Left+Math.Max(0,Math.Min(x,target.WorkingArea.Width-width));
        int screenY=target.WorkingArea.Top+Math.Max(0,Math.Min(y,target.WorkingArea.Height-height));
        return new Rectangle(screenX-(target.WorkingArea.Left-target.Bounds.Left),screenY-(target.WorkingArea.Top-target.Bounds.Top),width,height);
    }
    public static bool Move(WindowItem w,bool reveal,out string error){
        error="";if(!Native.IsWindow(w.Handle)||Native.Pid(w.Handle)!=w.Pid){error="窗口已经关闭";return false;}
        var source=Screen.FromHandle(w.Handle);var target=Screen.AllScreens.FirstOrDefault(s=>s.DeviceName!=source.DeviceName);
        if(target==null){error="没有检测到另一块显示器";return false;}
        var p=new Native.WINDOWPLACEMENT{Length=Marshal.SizeOf(typeof(Native.WINDOWPLACEMENT))};
        if(!Native.GetWindowPlacement(w.Handle,ref p)){error="无法读取窗口位置";return false;}
        bool minimized=Native.IsIconic(w.Handle),maximized=p.ShowCmd==3||(minimized&&(p.Flags&2)!=0);
        Rectangle normal=MapNormalRect(p.Normal.Rect,source,target);
        p.Normal=new Native.RECT{L=normal.Left,T=normal.Top,R=normal.Right,B=normal.Bottom};
        if(minimized)p.ShowCmd=7; // Never restore onto the source/shared monitor first.
        if(!Native.SetWindowPlacement(w.Handle,ref p)){error="移动失败，可能需要更高权限";return false;}
        if(!minimized&&maximized){var area=target.WorkingArea;Native.SetWindowPos(w.Handle,IntPtr.Zero,area.X,area.Y,area.Width,area.Height,0x14);}
        if(Screen.FromHandle(w.Handle).DeviceName!=target.DeviceName){error="未确认已移到另一屏，未执行恢复或激活";return false;}
        if(reveal){
            var popup=Native.GetLastActivePopup(w.Handle);
            if(popup!=IntPtr.Zero&&popup!=w.Handle&&Native.IsWindow(popup)&&Screen.FromHandle(popup).DeviceName!=target.DeviceName){
                Native.RECT dialog;if(Native.GetWindowRect(popup,out dialog)){var area=target.WorkingArea;Native.SetWindowPos(popup,IntPtr.Zero,area.X+Math.Max(0,(area.Width-dialog.Rect.Width)/2),area.Y+Math.Max(0,(area.Height-dialog.Rect.Height)/2),0,0,0x15);}
                if(Screen.FromHandle(popup).DeviceName!=target.DeviceName){error="关联对话框未能移到目标屏幕，未执行恢复或激活";return false;}
            }
            if(minimized)Native.ShowWindowAsync(w.Handle,maximized?3:9);
            Native.SetForegroundWindow(popup!=IntPtr.Zero&&Native.IsWindowVisible(popup)?popup:w.Handle);
        }
        return true;
    }
}
class Preferences {
    public bool Expanded=true;
    public int ExpandedWidth=240;
    public int CompactWidth=160;
    public int LayoutVersion=0;
    public bool Pinned=false;
    public int TopInset=96;
    public List<string> GroupOrder=new List<string>();
    public List<string> Collapsed=new List<string>();
    public Dictionary<string,string> GroupAliases=new Dictionary<string,string>();
    public Dictionary<string,string> WindowAliases=new Dictionary<string,string>();
    public Dictionary<string,List<string>> WindowOrder=new Dictionary<string,List<string>>();
}
class ShelfModel {
    public Preferences Pref;
    public List<WindowItem> Windows=new List<WindowItem>();
    public ShelfModel(Preferences p){Pref=p;}
    public List<string> Groups(){return Windows.Select(w=>w.Group).Distinct().OrderBy(g=>Index(Pref.GroupOrder,g)).ThenBy(g=>Windows.Find(w=>w.Group==g).App).ToList();}
    static int Index(List<string> list,string id){int i=list.IndexOf(id);return i<0?Int32.MaxValue:i;}
    public List<WindowItem> Children(string g){List<string> order;Pref.WindowOrder.TryGetValue(g,out order);return Windows.Where(w=>w.Group==g).OrderBy(w=>order==null?Int32.MaxValue:Index(order,w.Id)).ToList();}
    public string GroupLabel(string g){string label;return Pref.GroupAliases.TryGetValue(g,out label)?label:Windows.First(w=>w.Group==g).App;}
    public void Alias(WindowItem w,string text){w.Alias=String.IsNullOrWhiteSpace(text)?null:text.Trim();if(w.Alias==null)Pref.WindowAliases.Remove(w.Id);else Pref.WindowAliases[w.Id]=w.Alias;}
    public void Fold(string g){if(Pref.Collapsed.Contains(g))Pref.Collapsed.Remove(g);else Pref.Collapsed.Add(g);}
    public static void Move(List<string> order,string from,string to,bool after){if(from==to)return;order.Remove(from);int at=order.IndexOf(to);order.Insert(at<0?order.Count:at+(after?1:0),from);}
    public void MoveGroup(string from,string to,bool after){var list=new List<string>(Pref.GroupOrder);foreach(var g in Groups())if(!list.Contains(g))list.Add(g);Move(list,from,to,after);Pref.GroupOrder=list;}
    public void MoveWindow(string g,string from,string to,bool after){var list=Children(g).Select(w=>w.Id).ToList();Move(list,from,to,after);Pref.WindowOrder[g]=list;}
}
class ProcInfo {public string Path,Key,App,Started;}
class WindowCatalog : IDisposable {
    public int IconSize=32;
    Dictionary<int,ProcInfo> processes=new Dictionary<int,ProcInfo>();
    Dictionary<string,WindowItem> cache=new Dictionary<string,WindowItem>();
    public List<WindowItem> Scan(Preferences pref) {
        var found=new List<WindowItem>();var seenPids=new HashSet<int>();int own=Process.GetCurrentProcess().Id;
        Native.EnumWindows(delegate(IntPtr h,IntPtr unused){
            if(!Native.IsWindowVisible(h)||Native.Cloaked(h))return true;
            int pid=Native.Pid(h);if(pid==own||pid==0)return true;
            long ex=Native.GetWindowLongPtr(h,-20).ToInt64();
            if((ex&0x80)!=0 || (ex&0x08000000)!=0)return true;
            if(Native.GetWindow(h,4)!=IntPtr.Zero && (ex&0x40000)==0)return true;
            string cls=Native.Class(h);if(cls=="Shell_TrayWnd"||cls=="Shell_SecondaryTrayWnd"||cls=="Progman"||cls=="WorkerW")return true;
            string title=Native.Title(h);if(String.IsNullOrWhiteSpace(title))return true;
            try {
                ProcInfo info;if(!processes.TryGetValue(pid,out info)) {
                    string path=Native.PathOf(pid);string name=System.IO.Path.GetFileNameWithoutExtension(path);
                    if(name=="StartMenuExperienceHost"||name=="SearchHost"||name=="ShellExperienceHost"||name=="TextInputHost")return true;
                    string started="";try{using(var p=Process.GetProcessById(pid))started=p.StartTime.ToUniversalTime().Ticks.ToString();}catch{}
                    string aid=Native.AppId(pid);
                    string app=name;try{if(File.Exists(path)){var vi=FileVersionInfo.GetVersionInfo(path);if(!String.IsNullOrWhiteSpace(vi.ProductName))app=vi.ProductName;}}catch{}
                    if(String.IsNullOrWhiteSpace(app))app=cls;
                    info=new ProcInfo{Path=path,Key=String.IsNullOrEmpty(aid)?(String.IsNullOrEmpty(path)?cls:path.ToLowerInvariant()):aid,App=app,Started=started};processes[pid]=info;
                }
                seenPids.Add(pid);
                string id=pid+":"+info.Started+":"+h.ToInt64();WindowItem w;
                if(!cache.TryGetValue(id,out w)) {w=new WindowItem{Id=id,Handle=h,Pid=pid,Group=info.Key,App=info.App,Path=info.Path,Icon=Native.WindowIcon(h,info.Path,IconSize)};cache[id]=w;}
                w.Title=title;w.Secondary=Native.OnSecondaryMonitor(h);string alias;w.Alias=pref.WindowAliases.TryGetValue(id,out alias)?alias:null;found.Add(w);
            }catch{}return true;
        },IntPtr.Zero);
        var ids=new HashSet<string>(found.Select(w=>w.Id));
        foreach(var k in cache.Keys.Where(k=>!ids.Contains(k)).ToArray()){cache[k].Icon.Dispose();cache.Remove(k);}
        foreach(var k in processes.Keys.Where(k=>!seenPids.Contains(k)).ToArray())processes.Remove(k);
        return found;
    }
    public void Dispose(){foreach(var w in cache.Values)w.Icon.Dispose();cache.Clear();}
}
class ShelfRow {public string Group;public WindowItem Window;public bool Header;public Rectangle Bounds;}
class ShelfForm : Form {
    public ShelfModel Model;
    AppSettings settings; DateTime lastScan=DateTime.MinValue;
    WindowCatalog catalog=new WindowCatalog();
    List<ShelfRow> rows=new List<ShelfRow>();
    System.Windows.Forms.Timer timer=new System.Windows.Forms.Timer();
    ToolTip tip=new ToolTip(); NotifyIcon trayIcon;
    string statePath,diagnosticPath; bool testMode,paused,temporarilyHidden,hotkey,dragging,dragged; int tick,scroll,hover=-1,down=-1;Point downPoint;int drop=-1;bool dropAfter;
    Rectangle taskRect,trayRect;float scale=1;Font regular,bold,small;int rowHeight=40,headerHeight=34;IntPtr active;
    TextBox editor;ShelfRow editingRow;bool finishingEdit,hoverExpanded;int openMenus;DateTime enteredAt=DateTime.MinValue,leftAt=DateTime.MinValue;
    public ShelfForm(string dataPath,bool test){
        testMode=test;statePath=Path.Combine(dataPath,"preferences.json");diagnosticPath=Path.Combine(dataPath,"status.json");
        Directory.CreateDirectory(dataPath);settings=AppSettings.Load(dataPath,!test);catalog.IconSize=settings.IconSize;Model=new ShelfModel(LoadPreferences());
        if(Model.Pref.LayoutVersion<3){Model.Pref.CompactWidth=0;Model.Pref.ExpandedWidth=280;Model.Pref.Pinned=false;Model.Pref.LayoutVersion=3;}
        if(Model.Pref.LayoutVersion<4){Model.Pref.TopInset=0;Model.Pref.LayoutVersion=4;}
        Text="Window Shelf · 窗口侧栏";FormBorderStyle=FormBorderStyle.None;ShowInTaskbar=Program.UiVerification;TopMost=true;AutoScaleMode=AutoScaleMode.None;BackColor=C(settings.Colors.Background,"#F3F4F6");
        SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.UserPaint|ControlStyles.ResizeRedraw,true);
        SetFonts(1);MinimumSize=new Size(70,90);KeyPreview=true;
        tip.InitialDelay=500;tip.ReshowDelay=150;tip.AutoPopDelay=12000;
        MouseWheel+=delegate(object s,MouseEventArgs e){scroll=Math.Max(0,Math.Min(MaxScroll(),scroll-e.Delta/120*rowHeight*3));BuildRows();Invalidate();};
        KeyDown+=delegate(object s,KeyEventArgs e){if(e.KeyCode==Keys.Escape){dragging=false;Capture=false;Invalidate();}if(e.KeyCode==Keys.F2&&hover>=0&&hover<rows.Count)Rename(rows[hover]);};
        if(!test){
            trayIcon=new NotifyIcon{Icon=SystemIcons.Application,Text="窗口侧栏 · 右键可暂停或退出",Visible=true};
            trayIcon.MouseClick+=delegate(object s,MouseEventArgs e){if(e.Button==MouseButtons.Left){paused=!paused;UpdatePlacement();}else if(e.Button==MouseButtons.Right)ShowMainMenu(Cursor.Position);};
            timer.Interval=300;timer.Tick+=delegate{Tick();};
            Shown+=delegate { hotkey=Native.RegisterHotKey(Handle,1,0x4000|2|4,(uint)Keys.F12);RefreshWindows();UpdatePlacement();timer.Start(); };
        }
    }
    protected override bool ShowWithoutActivation {get{return true;}}
    protected override CreateParams CreateParams {get{var c=base.CreateParams;if(!Program.UiVerification)c.ExStyle|=0x80;return c;}}
    Preferences LoadPreferences(){try{if(File.Exists(statePath)){var p=new JavaScriptSerializer().Deserialize<Preferences>(File.ReadAllText(statePath));if(p!=null)return p;}}catch{}return new Preferences();}
    void Save(){if(testMode)return;try{string t=statePath+".new";File.WriteAllText(t,new JavaScriptSerializer().Serialize(Model.Pref),new UTF8Encoding(false));if(File.Exists(statePath))File.Replace(t,statePath,null);else File.Move(t,statePath);}catch{tip.Show("设置暂时未能保存",this,8,headerHeight,2500);}}
    void SetFonts(float s){scale=s;rowHeight=(int)Math.Round(settings.RowHeight*s);headerHeight=(int)Math.Round(settings.HeaderHeight*s);if(regular!=null){regular.Dispose();bold.Dispose();small.Dispose();}regular=new Font(settings.FontFamily,settings.FontSize*s,FontStyle.Regular,GraphicsUnit.Pixel);bold=new Font(settings.FontFamily,settings.FontSize*s,FontStyle.Bold,GraphicsUnit.Pixel);small=new Font("Microsoft YaHei UI",12*s,FontStyle.Regular,GraphicsUnit.Pixel);Font=regular;}
    int S(int v){return (int)Math.Round(v*scale);}
    static Color C(string hex,string fallback){return ShelfPalette.Parse(hex,fallback);}
    void ReloadSettings(){if(editor!=null)FinishRename(true);settings=AppSettings.Load(Path.GetDirectoryName(statePath),true);catalog.Dispose();catalog=new WindowCatalog{IconSize=settings.IconSize};BackColor=C(settings.Colors.Background,"#F3F4F6");SetFonts(scale);RefreshWindows();UpdatePlacement();Invalidate();}
    void Tick(){if(IsDisposed)return;try{tick++;if((DateTime.UtcNow-lastScan).TotalMilliseconds>=settings.RefreshIntervalMs&&!dragging&&editor==null){lastScan=DateTime.UtcNow;RefreshWindows();}UpdatePlacement();if(tick%6==0)WriteStatus();}catch(Exception e){WriteError(e);}}
    void WriteError(Exception e){try{File.WriteAllText(Path.Combine(Path.GetDirectoryName(statePath),"last-error.txt"),e.GetType().Name+": "+e.Message);}catch{}}
    public void RefreshWindows(){Model.Windows=catalog.Scan(Model.Pref);bool changed=false;
        foreach(var g in Model.Windows.Select(w=>w.Group).Distinct()){
            if(!Model.Pref.GroupOrder.Contains(g)){Model.Pref.GroupOrder.Add(g);changed=true;}
            List<string> order;if(!Model.Pref.WindowOrder.TryGetValue(g,out order)){order=new List<string>();Model.Pref.WindowOrder[g]=order;}
            foreach(var w in Model.Windows.Where(w=>w.Group==g))if(!order.Contains(w.Id)){order.Add(w.Id);changed=true;}
        }
        var ids=new HashSet<string>(Model.Windows.Select(w=>w.Id));
        foreach(var order in Model.Pref.WindowOrder.Values)if(order.RemoveAll(id=>!ids.Contains(id))>0)changed=true;
        foreach(var id in Model.Pref.WindowAliases.Keys.Where(id=>!ids.Contains(id)).ToArray()){Model.Pref.WindowAliases.Remove(id);changed=true;}
        if(changed)Save();BuildRows();Invalidate();}
    bool GetGeometry(){
        var h=Native.FindWindow("Shell_TrayWnd",null);Native.RECT r,tr;
        if(h==IntPtr.Zero||!Native.IsWindowVisible(h)||!Native.GetWindowRect(h,out r))return false;
        var th=Native.FindWindowEx(h,IntPtr.Zero,"TrayNotifyWnd",null);if(th==IntPtr.Zero||!Native.GetWindowRect(th,out tr))return false;
        taskRect=r.Rect;trayRect=tr.Rect;var screen=Screen.FromHandle(h).Bounds;
        if(taskRect.Width>=taskRect.Height||Math.Abs(taskRect.Left-screen.Left)>4||trayRect.Top<=taskRect.Top||trayRect.Height<30)return false;
        float dpi=Native.GetDpiForWindow(h)/96f;if(dpi>0&&Math.Abs(dpi-scale)>.02)SetFonts(dpi);
        return true;
    }
    bool FullScreenOrShellMenu(){
        IntPtr h=Native.GetForegroundWindow();if(h==IntPtr.Zero||Native.Pid(h)==Process.GetCurrentProcess().Id)return false;
        string cls=Native.Class(h);if(cls=="Shell_TrayWnd"||cls=="Shell_SecondaryTrayWnd"||cls=="Progman"||cls=="WorkerW")return false;
        string path=Native.PathOf(Native.Pid(h));string p=Path.GetFileNameWithoutExtension(path);
        if(p=="StartMenuExperienceHost"||p=="SearchHost")return true;
        Native.RECT r;if(!Native.GetWindowRect(h,out r))return false;var b=Screen.FromRectangle(taskRect).Bounds;var rr=r.Rect;
        return rr.Left<=b.Left&&rr.Top<=b.Top&&rr.Right>=b.Right&&rr.Bottom>=b.Bottom;
    }
    public void UpdatePlacement(){if(testMode)return;
        bool valid=GetGeometry();temporarilyHidden=!valid||FullScreenOrShellMenu();
        if(paused||temporarilyHidden){if(Visible)Hide();return;}
        int top=taskRect.Top+S(settings.TopInset),bottom=trayRect.Top-S(3);
        if(bottom-top<headerHeight+rowHeight){if(Visible)Hide();return;}
        bool inside=Bounds.Contains(Cursor.Position)||editor!=null||openMenus>0||dragging;
        DateTime now=DateTime.UtcNow;
        if(inside){leftAt=DateTime.MinValue;if(enteredAt==DateTime.MinValue)enteredAt=now;if((now-enteredAt).TotalMilliseconds>=settings.HoverExpandDelayMs)hoverExpanded=true;}
        else{enteredAt=DateTime.MinValue;if(leftAt==DateTime.MinValue)leftAt=now;if((now-leftAt).TotalMilliseconds>=settings.HoverCollapseDelayMs)hoverExpanded=false;}
        int width=Model.Pref.Pinned||hoverExpanded?Math.Max(taskRect.Width,S(settings.ExpandedWidth)):taskRect.Width;
        var rect=new Rectangle(taskRect.Left,top,width,bottom-top);
        if(Bounds!=rect){Bounds=rect;BuildRows();PlaceEditor();}
        if(!Visible)Show();
        // Reassert only when the native taskbar was activated, not above arbitrary menus.
        if(Native.GetForegroundWindow()==Native.FindWindow("Shell_TrayWnd",null))Native.SetWindowPos(Handle,new IntPtr(-1),0,0,0,0,0x13);
        IntPtr foreground=Native.GetForegroundWindow();IntPtr a=Native.GetAncestor(foreground,3);if(Native.Pid(foreground)!=Process.GetCurrentProcess().Id&&active!=a){active=a;Invalidate();}
    }
    public void BuildRows(){
        rows.Clear();
        foreach(string g in Model.Groups()){
            var children=Model.Children(g);if(children.Count>1){rows.Add(new ShelfRow{Group=g,Header=true});if(Model.Pref.Collapsed.Contains(g))continue;}
            foreach(var w in children)rows.Add(new ShelfRow{Group=g,Window=w});
        }
        scroll=Math.Min(scroll,MaxScroll());int contentHeight=rows.Count*rowHeight;
        int y=headerHeight+(settings.CenterList?Math.Max(0,(ClientSize.Height-headerHeight-contentHeight)/2):0)-scroll;
        foreach(var row in rows){row.Bounds=new Rectangle(0,y,ClientSize.Width,rowHeight);y+=rowHeight;}
    }
    int MaxScroll(){return Math.Max(0,rows.Count*rowHeight-(ClientSize.Height-headerHeight));}
    Rectangle CloseRect(ShelfRow r){return ClientSize.Width<S(150)||r.Header?Rectangle.Empty:new Rectangle(ClientSize.Width-S(settings.CloseButtonWidth+2),r.Bounds.Top,S(settings.CloseButtonWidth),rowHeight);}
    int IconRailWidth(){return taskRect.Width>0?taskRect.Width:S(92);}
    bool DetailRow(ShelfRow r){return !r.Header&&Model.Children(r.Group).Count>1;}
    bool RowVisible(ShelfRow r){return ClientSize.Width>=S(150)||!DetailRow(r);}
    Rectangle IconRect(ShelfRow r){int x=(IconRailWidth()-S(settings.IconSize))/2+(DetailRow(r)?S(settings.ChildIndent):0);return new Rectangle(x,r.Bounds.Top+(rowHeight-S(settings.IconSize))/2,S(settings.IconSize),S(settings.IconSize));}
    bool SecondaryRow(ShelfRow r){return r.Header?Model.Children(r.Group).Any(w=>w.Secondary):r.Window.Secondary;}
    int LabelX(ShelfRow r){return IconRailWidth()+S(r.Header?12:DetailRow(r)?8+settings.ChildIndent:8);}
    void DrawAppIcon(Graphics g,Bitmap icon,int x,int y){if(icon==null)return;int size=S(settings.IconSize);if(icon.Width==size&&icon.Height==size){g.DrawImageUnscaled(icon,x,y);return;}var state=g.Save();g.InterpolationMode=System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;g.PixelOffsetMode=PixelOffsetMode.Half;g.DrawImage(icon,new Rectangle(x,y,size,size),new Rectangle(0,0,icon.Width,icon.Height),GraphicsUnit.Pixel);g.Restore(state);}
    int Hit(Point p){if(p.Y<headerHeight)return -1;for(int i=0;i<rows.Count;i++)if(RowVisible(rows[i])&&(!DetailRow(rows[i])||p.X>=S(settings.ChildIndent))&&rows[i].Bounds.Contains(p))return i;return -1;}
    void TextAt(Graphics g,string text,Font f,Rectangle rect,Color color){if(rect.Width>0)TextRenderer.DrawText(g,text,f,rect,color,TextFormatFlags.EndEllipsis|TextFormatFlags.VerticalCenter|TextFormatFlags.SingleLine|TextFormatFlags.NoPrefix|TextFormatFlags.NoPadding);}
    protected override void OnPaint(PaintEventArgs e){
        base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;bool narrow=ClientSize.Width<S(150);
        using(var border=new Pen(C(settings.Colors.Border,"#D8DDE4")))g.DrawLine(border,ClientSize.Width-1,0,ClientSize.Width-1,Height);
        var clip=g.Save();g.SetClip(new Rectangle(0,headerHeight,Width,Math.Max(0,Height-headerHeight)));
        for(int i=0;i<rows.Count;i++){
            var r=rows[i];if(!RowVisible(r)||r.Bounds.Bottom<headerHeight||r.Bounds.Top>Height)continue;
            var rowState=g.Save();if(DetailRow(r))g.SetClip(new Rectangle(S(settings.ChildIndent),headerHeight,Math.Max(0,Width-S(settings.ChildIndent)),Math.Max(0,Height-headerHeight)),CombineMode.Intersect);
            bool selected=r.Window!=null&&r.Window.Handle==active;
            bool secondary=SecondaryRow(r);
            if(secondary){using(var b=new SolidBrush(selected?C(settings.Colors.SecondaryActive,"#C5D6EC"):C(settings.Colors.Secondary,"#DCE5F2")))g.FillRectangle(b,new Rectangle(3,r.Bounds.Y+1,Width-7,rowHeight-2));
                if(r.Header&&Model.Children(r.Group).Any(w=>!w.Secondary)){using(var b=new SolidBrush(C(settings.Colors.Background,"#F3F4F6")))g.FillRectangle(b,new Rectangle(3,r.Bounds.Y+1,(Width-7)/2,rowHeight-2));}
            }else if(selected||i==hover){using(var b=new SolidBrush(selected?C(settings.Colors.Active,"#E2E7ED"):C(settings.Colors.Hover,"#E9EDF2")))g.FillRectangle(b,new Rectangle(3,r.Bounds.Y+1,Width-7,rowHeight-2));}
            if(selected){using(var b=new SolidBrush(C(settings.Colors.Accent,"#56759A")))g.FillRectangle(b,new Rectangle(1,r.Bounds.Y+S(7),S(3),rowHeight-S(14)));}
            var fg=C(settings.Colors.Text,"#303844");var items=Model.Children(r.Group);var iconBounds=IconRect(r);DrawAppIcon(g,r.Header?items[0].Icon:r.Window.Icon,iconBounds.X,iconBounds.Y);
            if(r.Header)TextAt(g,items.Count.ToString(),bold,new Rectangle(iconBounds.Right+S(4),r.Bounds.Y,Math.Max(0,IconRailWidth()-iconBounds.Right-S(6)),rowHeight),C(settings.Colors.Close,"#66717E"));
            if(narrow){g.Restore(rowState);continue;}
            int x=LabelX(r);
            if(r.Header){
                TextAt(g,Model.Pref.Collapsed.Contains(r.Group)?"▸":"▾",regular,new Rectangle(IconRailWidth()-S(4),r.Bounds.Y,S(14),rowHeight),fg);
                string label=Model.GroupLabel(r.Group);
                TextAt(g,label,bold,new Rectangle(x,r.Bounds.Y,Width-x-S(8),rowHeight),fg);
            }else{
                string display=r.Window.Label;
                TextAt(g,display,regular,new Rectangle(x,r.Bounds.Y,Width-x-S(settings.CloseButtonWidth+5),rowHeight),fg);
                var cr=CloseRect(r);int cx=cr.X+cr.Width/2,cy=cr.Y+cr.Height/2;using(var pen=new Pen(i==hover?C(settings.Colors.CloseHover,"#A53841"):C(settings.Colors.Close,"#66717E"),S(2))){g.DrawLine(pen,cx-S(7),cy-S(7),cx+S(7),cy+S(7));g.DrawLine(pen,cx+S(7),cy-S(7),cx-S(7),cy+S(7));}
            }
            g.Restore(rowState);
        }
        if(dragging&&drop>=0&&drop<rows.Count){int yy=dropAfter?rows[drop].Bounds.Bottom:rows[drop].Bounds.Top;using(var p=new Pen(C(settings.Colors.Accent,"#56759A"),2))g.DrawLine(p,5,yy,Width-6,yy);}
        g.Restore(clip);
        using(var b=new SolidBrush(C(settings.Colors.Header,"#E9ECF0")))g.FillRectangle(b,0,0,Width,headerHeight);
        TextAt(g,narrow?Model.Windows.Count.ToString():"窗口  "+Model.Windows.Count,bold,new Rectangle(S(10),0,Width-S(65),headerHeight),C(settings.Colors.Text,"#303844"));
        TextAt(g,Model.Pref.Pinned?"•":"↔",bold,new Rectangle(Width-S(56),0,S(25),headerHeight),C(settings.Colors.Text,"#303844"));
        TextAt(g,"⋯",bold,new Rectangle(Width-S(27),0,S(25),headerHeight),C(settings.Colors.Text,"#303844"));
        if(rows.Count==0)TextAt(g,"暂无窗口",regular,new Rectangle(S(8),headerHeight+S(12),Width-S(16),rowHeight),Color.Gray);
        if(MaxScroll()>0){int h=Math.Max(S(24),(Height-headerHeight)*(Height-headerHeight)/(rows.Count*rowHeight));int y=headerHeight+(Height-headerHeight-h)*scroll/MaxScroll();using(var b=new SolidBrush(C(settings.Colors.Close,"#66717E")))g.FillRectangle(b,Width-3,y,2,h);}
    }
    protected override void OnMouseMove(MouseEventArgs e){base.OnMouseMove(e);int h=Hit(e.Location);
        if(h!=hover){hover=h;tip.SetToolTip(this,h<0?"↔ 自动展开 / 固定展开 · ⋯ 菜单 · Ctrl+Alt+F12 暂停覆盖":(rows[h].Header?Model.GroupLabel(rows[h].Group)+"（"+Model.Children(rows[h].Group).Count+" 个窗口，副屏 "+Model.Children(rows[h].Group).Count(w=>w.Secondary)+" 个）":rows[h].Window.Label+(rows[h].Window.Secondary?"（副屏）":"")+(String.IsNullOrEmpty(rows[h].Window.Alias)?"":"\n原标题："+rows[h].Window.Title)+"\n右键原位重命名；× 关闭窗口"));Invalidate();}
        if(down>=0&&e.Button==MouseButtons.Left){
            if(!dragging&&(Math.Abs(e.X-downPoint.X)>S(5)||Math.Abs(e.Y-downPoint.Y)>S(5))){dragging=true;dragged=true;Capture=true;tip.Hide(this);}
            if(dragging){if(e.Y>Height-S(24))scroll=Math.Min(MaxScroll(),scroll+rowHeight/2);if(e.Y<headerHeight+S(12))scroll=Math.Max(0,scroll-rowHeight/2);BuildRows();drop=Hit(e.Location);dropAfter=drop>=0&&e.Y>rows[drop].Bounds.Top+rowHeight/2;Invalidate();}
        }
    }
    protected override void OnMouseLeave(EventArgs e){base.OnMouseLeave(e);if(!dragging){hover=-1;Invalidate();}}
    protected override void OnMouseDown(MouseEventArgs e){base.OnMouseDown(e);if(editor!=null&&!editor.Bounds.Contains(e.Location))FinishRename(true);down=-1;dragged=false;if(e.Button!=MouseButtons.Left||e.Y<headerHeight)return;int h=Hit(e.Location);if(h>=0&&(rows[h].Header||!CloseRect(rows[h]).Contains(e.Location))){down=h;downPoint=e.Location;}}
    protected override void OnMouseUp(MouseEventArgs e){base.OnMouseUp(e);int h=Hit(e.Location);
        if(dragging){if(down>=0&&down<rows.Count&&drop>=0&&drop<rows.Count){var from=rows[down];var to=rows[drop];if(from.Header||Model.Children(from.Group).Count==1)Model.MoveGroup(from.Group,to.Group,dropAfter);else if(!to.Header&&from.Group==to.Group)Model.MoveWindow(from.Group,from.Window.Id,to.Window.Id,dropAfter);Save();}dragging=false;Capture=false;drop=-1;down=-1;BuildRows();Invalidate();return;}
        down=-1;if(dragged)return;
        if(e.Button==MouseButtons.Right){if(h>=0)ShowRowMenu(rows[h],PointToScreen(e.Location));else ShowMainMenu(PointToScreen(e.Location));return;}
        if(e.Button!=MouseButtons.Left)return;
        if(e.Y<headerHeight){if(e.X>Width-S(29))ShowMainMenu(PointToScreen(e.Location));else if(e.X>Width-S(59))ToggleWidth();return;}
        if(h<0)return;var row=rows[h];if(row.Header){if(ClientSize.Width<S(150)){Model.Pref.Collapsed.Remove(row.Group);hoverExpanded=true;UpdatePlacement();}else Model.Fold(row.Group);Save();BuildRows();Invalidate();}else if(CloseRect(row).Contains(e.Location))CloseWindow(row.Window);else ActivateWindow(row.Window);
    }
    void ToggleWidth(){Model.Pref.Pinned=!Model.Pref.Pinned;Save();UpdatePlacement();BuildRows();Invalidate();}
    internal static bool ShouldMinimize(bool minimized,bool foreground){return !minimized&&foreground;}
    void ActivateWindow(WindowItem w){
        if(!Native.IsWindow(w.Handle)||Native.Pid(w.Handle)!=w.Pid)return;
        bool minimized=Native.IsIconic(w.Handle);
        bool foreground=Native.GetAncestor(Native.GetForegroundWindow(),3)==w.Handle;
        if(ShouldMinimize(minimized,foreground)){Native.ShowWindowAsync(w.Handle,6);return;}
        var h=Native.GetLastActivePopup(w.Handle);if(h==IntPtr.Zero||!Native.IsWindowVisible(h))h=w.Handle;
        if(minimized){Native.ShowWindowAsync(w.Handle,9);h=w.Handle;}
        if(!Native.SetForegroundWindow(h))tip.Show("窗口未能切到前台，请检查是否有对话框",this,8,headerHeight,2400);
    }
    void CloseWindow(WindowItem w){if(testMode)return;if(!Native.IsWindow(w.Handle)||Native.Pid(w.Handle)!=w.Pid)return;if(!Native.PostMessage(w.Handle,0x10,IntPtr.Zero,IntPtr.Zero))tip.Show("未能关闭：该窗口可能需要更高权限",this,8,headerHeight,3000);}
    void MoveToOtherScreen(WindowItem w){string error;if(!WindowMover.Move(w,true,out error))tip.Show(error,this,8,headerHeight,3500);else{RefreshWindows();Invalidate();}}
    void Rename(ShelfRow r){
        if(editor!=null)FinishRename(true);editingRow=r;hoverExpanded=true;UpdatePlacement();
        editor=new TextBox{Text=r.Header?Model.GroupLabel(r.Group):r.Window.Label,Font=regular,BorderStyle=BorderStyle.FixedSingle,BackColor=Color.White,ForeColor=C(settings.Colors.Text,"#303844")};
        Controls.Add(editor);PlaceEditor();tip.Hide(this);
        editor.KeyDown+=delegate(object sender,KeyEventArgs e){if(e.KeyCode==Keys.Enter||e.KeyCode==Keys.Escape){e.SuppressKeyPress=true;e.Handled=true;FinishRename(e.KeyCode==Keys.Enter);}};
        editor.LostFocus+=delegate{if(!finishingEdit)FinishRename(true);};
        if(!testMode)BeginInvoke(new Action(delegate{if(editor!=null){Activate();editor.Focus();editor.SelectAll();}}));
    }
    void PlaceEditor(){if(editor==null||editingRow==null)return;var r=rows.FirstOrDefault(item=>item.Group==editingRow.Group&&item.Header==editingRow.Header&&(item.Header||item.Window.Id==editingRow.Window.Id));if(r==null)return;editingRow=r;int x=LabelX(r);editor.SetBounds(x,r.Bounds.Y+S(6),Math.Max(S(30),Width-x-S(r.Header?7:settings.CloseButtonWidth+5)),S(28));}
    void FinishRename(bool save){if(editor==null||finishingEdit)return;finishingEdit=true;var control=editor;var row=editingRow;string text=control.Text;editor=null;editingRow=null;
        if(save){if(row.Header){if(String.IsNullOrWhiteSpace(text))Model.Pref.GroupAliases.Remove(row.Group);else Model.Pref.GroupAliases[row.Group]=text.Trim();}else Model.Alias(row.Window,text);Save();}
        Controls.Remove(control);control.Dispose();finishingEdit=false;Invalidate();
    }
    ContextMenuStrip MakeMenu(){var m=new ContextMenuStrip{Font=new Font("Microsoft YaHei UI",10)};m.Opened+=delegate{openMenus++;};m.Closed+=delegate{openMenus=Math.Max(0,openMenus-1);};return m;}
    void ShowRowMenu(ShelfRow r,Point p){var m=MakeMenu();m.Items.Add("重命名…",null,delegate{Rename(r);});m.Items.Add(r.Header?"恢复应用名称":"恢复原标题（持续跟随）",null,delegate{if(r.Header)Model.Pref.GroupAliases.Remove(r.Group);else Model.Alias(r.Window,null);Save();Invalidate();});
        m.Items.Add(new ToolStripSeparator());if(r.Header)m.Items.Add(Model.Pref.Collapsed.Contains(r.Group)?"展开":"折叠",null,delegate{Model.Fold(r.Group);Save();BuildRows();Invalidate();});
        else{m.Items.Add("在另一块显示器显示",null,delegate{MoveToOtherScreen(r.Window);});m.Items.Add("切换到窗口",null,delegate{ActivateWindow(r.Window);});m.Items.Add("最小化窗口",null,delegate{if(Native.IsWindow(r.Window.Handle)&&Native.Pid(r.Window.Handle)==r.Window.Pid)Native.ShowWindowAsync(r.Window.Handle,6);});m.Items.Add("关闭窗口",null,delegate{CloseWindow(r.Window);});}
        m.Show(p);
    }
    void ShowMainMenu(Point p){var m=MakeMenu();m.Items.Add(Model.Pref.Pinned?"恢复悬停展开":"固定保持展开",null,delegate{ToggleWidth();});
        m.Items.Add(new ToolStripMenuItem("雾蓝底：副屏窗口；双色：跨屏分组"){Enabled=false});
        m.Items.Add("全部展开",null,delegate{Model.Pref.Collapsed.Clear();Save();BuildRows();Invalidate();});
        m.Items.Add("全部折叠",null,delegate{Model.Pref.Collapsed=Model.Groups();Save();BuildRows();Invalidate();});
        m.Items.Add(new ToolStripSeparator());m.Items.Add(paused?"恢复覆盖":"暂停覆盖，显示原任务栏",null,delegate{paused=!paused;UpdatePlacement();});
        m.Items.Add("打开设置文件",null,delegate{Process.Start(new ProcessStartInfo(Path.Combine(Path.GetDirectoryName(statePath),"settings.json")){UseShellExecute=true});});m.Items.Add("重新加载设置",null,delegate{ReloadSettings();});m.Items.Add("退出窗口侧栏",null,delegate{Close();});m.Show(p);
    }
    protected override void WndProc(ref Message m){
        // Keep the current app in the foreground until the row decides whether to minimize it.
        if(m.Msg==0x21&&editor==null){m.Result=new IntPtr(3);return;}
        if(m.Msg==0x312&&m.WParam.ToInt32()==1){paused=!paused;UpdatePlacement();}base.WndProc(ref m);
    }
    protected override void OnFormClosed(FormClosedEventArgs e){timer.Stop();Save();if(hotkey)Native.UnregisterHotKey(Handle,1);if(trayIcon!=null){trayIcon.Visible=false;trayIcon.Dispose();}catalog.Dispose();tip.Dispose();base.OnFormClosed(e);}
    public void WriteStatus(){try{File.WriteAllText(diagnosticPath,new JavaScriptSerializer().Serialize(new {utc=DateTime.UtcNow.ToString("o"),visible=Visible,paused=paused,boundaryValid=!temporarilyHidden,bounds=new {x=Left,y=Top,width=Width,height=Height},tray=new {x=trayRect.X,y=trayRect.Y,width=trayRect.Width,height=trayRect.Height},windowCount=Model.Windows.Count,groupCount=Model.Groups().Count,rowCount=rows.Count,expanded=Model.Pref.Pinned||hoverExpanded,pinned=Model.Pref.Pinned,inlineEditing=editor!=null,compactWidth=Model.Pref.CompactWidth,expandedWidth=settings.ExpandedWidth,hotkey=hotkey,rows=rows.Select(r=>new{header=r.Header,handle=(r.Header?Model.Children(r.Group)[0].Handle:r.Window.Handle).ToInt64(),y=r.Bounds.Y,height=r.Bounds.Height,alias=!r.Header&&!String.IsNullOrEmpty(r.Window.Alias)}).ToArray(),fixtures=Model.Windows.Where(w=>w.Title.StartsWith("WindowShelf 验证窗口")).Select(w=>new{handle=w.Handle.ToInt64(),title=w.Title,label=w.Label}).ToArray()}),new UTF8Encoding(false));}catch{}}
    public void RunUiTests(List<string> results){
        Assert(ShouldMinimize(false,true),"foreground row click minimizes",results);
        Assert(!ShouldMinimize(true,true),"minimized row click restores",results);
        Assert(!ShouldMinimize(false,false),"background row click activates",results);
        Message activation=Message.Create(Handle,0x21,IntPtr.Zero,IntPtr.Zero);WndProc(ref activation);Assert(activation.Result==new IntPtr(3),"row click does not steal foreground before toggle",results);
        Bounds=new Rectangle(320,160,240,350);Model.Windows=new List<WindowItem>{new WindowItem{Id="a1",Group="a",App="Word",Title="文档甲",Icon=SystemIcons.Application.ToBitmap()},new WindowItem{Id="a2",Group="a",App="Word",Title="文档乙",Icon=SystemIcons.Application.ToBitmap()},new WindowItem{Id="b1",Group="b",App="Code",Title="项目",Icon=SystemIcons.Application.ToBitmap()}};
        Model.Pref.GroupOrder=new List<string>{"a","b"};
        BuildRows();Assert(rows.Count==4,"group rows",results);
        Rename(rows[1]);Assert(editor!=null&&editor.Parent==this&&editor.Bounds.IntersectsWith(rows[1].Bounds),"rename editor stays inside original row",results);editor.Text="行内编辑验证";FinishRename(true);Assert(Model.Windows[0].Alias=="行内编辑验证","inline rename commits",results);
        Rename(rows[1]);editor.Text="不应保存";FinishRename(false);Assert(Model.Windows[0].Alias=="行内编辑验证","cancel inline rename preserves prior alias",results);
        Rename(rows[1]);editor.Text="";FinishRename(true);Assert(Model.Windows[0].Alias==null,"empty inline name follows title",results);
        Model.Alias(Model.Windows[0],"论文正文");Model.Windows[0].Title="文档甲 - 已更新";Assert(Model.Windows[0].Label=="论文正文","alias survives live title change",results);
        Model.Alias(Model.Windows[0],null);Assert(Model.Windows[0].Label=="文档甲 - 已更新","restore follows latest title",results);
        OnMouseUp(new MouseEventArgs(MouseButtons.Left,1,12,rows[0].Bounds.Y+10,0));Assert(rows.Count==2,"click collapse",results);
        OnMouseUp(new MouseEventArgs(MouseButtons.Left,1,12,rows[0].Bounds.Y+10,0));Assert(rows.Count==4,"click expand",results);
        int y1=rows[1].Bounds.Y+10,y2=rows[2].Bounds.Bottom-4;
        int detailX=LabelX(rows[1])+2;OnMouseDown(new MouseEventArgs(MouseButtons.Left,1,detailX,y1,0));OnMouseMove(new MouseEventArgs(MouseButtons.Left,0,detailX,y2,0));OnMouseUp(new MouseEventArgs(MouseButtons.Left,1,detailX,y2,0));Assert(Model.Children("a")[0].Id=="a2","mouse drag window order",results);
        int yb=rows[3].Bounds.Y+10,yt=rows[0].Bounds.Y+2;OnMouseDown(new MouseEventArgs(MouseButtons.Left,1,50,yb,0));OnMouseMove(new MouseEventArgs(MouseButtons.Left,0,50,yt,0));OnMouseUp(new MouseEventArgs(MouseButtons.Left,1,50,yt,0));Assert(Model.Groups()[0]=="b","mouse drag group order",results);
        Model.Fold("a");BuildRows();Assert(rows.Count==2,"fold survives reorder",results);
        var serialized=new JavaScriptSerializer().Serialize(Model.Pref);var restored=new JavaScriptSerializer().Deserialize<Preferences>(serialized);Assert(restored.GroupOrder[0]=="b"&&restored.Collapsed.Contains("a"),"saved layout roundtrip",results);
        Model.Pref.Collapsed.Clear();BuildRows();ClientSize=new Size(240,100);BuildRows();Assert(MaxScroll()>0,"overflow scroll instead of shrink",results);Assert(rowHeight==44,"larger row height remains fixed",results);
        ClientSize=new Size(280,700);BuildRows();foreach(var r in rows.Where(r=>!r.Header))Assert(CloseRect(r).Right<=Width&&CloseRect(r).Width>=40,"large close target inside expanded row",results);
        Assert(Math.Abs((rows[0].Bounds.Top-headerHeight)-(ClientSize.Height-rows.Last().Bounds.Bottom))<=1,"expanded list centered",results);
        var expandedIcons=rows.Where(r=>!DetailRow(r)).ToDictionary(r=>r.Group,r=>IconRect(r));
        ClientSize=new Size(92,700);BuildRows();Assert(rows.Count(RowVisible)==2,"compact mode has one icon per application",results);
        foreach(var r in rows.Where(RowVisible)){Assert(CloseRect(r).IsEmpty,"compact mode has no close target",results);var rect=IconRect(r);Assert(Math.Abs((rect.Left+rect.Right)-ClientSize.Width)<=1,"compact icon centered",results);Assert(rect==expandedIcons[r.Group],"hover resize preserves icon x and y",results);Assert(Hit(new Point(rect.X+16,rect.Y+16))==rows.IndexOf(r),"compact icon click target matches expanded position",results);}
        foreach(var r in rows.Where(DetailRow))Assert(Hit(new Point(IconRailWidth()/2,r.Bounds.Y+rowHeight/2))==-1,"reserved detail slot is not clickable in compact rail",results);
        Assert(Math.Abs((rows[0].Bounds.Top-headerHeight)-(ClientSize.Height-rows.Last().Bounds.Bottom))<=1,"compact list centered",results);
        Model.Windows[0].Secondary=true;Assert(SecondaryRow(rows.First(r=>r.Group=="a")),"mixed monitor group marked",results);
        ClientSize=new Size(280,700);BuildRows();Assert(SecondaryRow(rows.First(r=>r.Window!=null&&r.Window.Id=="a1")),"secondary window marked",results);Assert(!SecondaryRow(rows.First(r=>r.Window!=null&&r.Window.Id=="a2")),"primary window not marked",results);
        foreach(var r in rows.Where(r=>!DetailRow(r)))Assert(IconRect(r)==expandedIcons[r.Group],"expand roundtrip does not move application icon",results);
        ClientSize=new Size(92,100);BuildRows();scroll=Math.Min(rowHeight,MaxScroll());BuildRows();int priorScroll=scroll;var scrolled=rows.Where(r=>!DetailRow(r)).ToDictionary(r=>r.Group,r=>IconRect(r));ClientSize=new Size(280,100);BuildRows();Assert(scroll==priorScroll,"hover preserves scroll offset",results);foreach(var r in rows.Where(r=>!DetailRow(r)))Assert(IconRect(r)==scrolled[r.Group],"overflow hover preserves icon positions",results);
        ClientSize=new Size(280,700);BuildRows();
    }
    public void RunMonitorTests(List<string> results){
        if(Screen.AllScreens.Length<2){results.Add("SKIP: second monitor unavailable");return;}
        using(var fixture=new Form{Text="WindowShelf 跨屏验证",ShowInTaskbar=false,StartPosition=FormStartPosition.Manual,Bounds=new Rectangle(Screen.PrimaryScreen.WorkingArea.X+180,Screen.PrimaryScreen.WorkingArea.Y+180,360,180)}){
            var w=new WindowItem{Handle=fixture.Handle,Pid=Process.GetCurrentProcess().Id};string error;
            Assert(!Native.OnSecondaryMonitor(w.Handle),"primary monitor detected",results);
            Assert(WindowMover.Move(w,false,out error),"normal window move: "+error,results);Assert(Native.OnSecondaryMonitor(w.Handle),"secondary monitor detected after move",results);
            Native.ShowWindowAsync(w.Handle,7);for(int i=0;i<30&&!Native.IsIconic(w.Handle);i++){Application.DoEvents();Thread.Sleep(10);}
            Assert(Native.IsIconic(w.Handle),"fixture minimized before transfer",results);
            Assert(WindowMover.Move(w,false,out error),"minimized transfer: "+error,results);Assert(Native.IsIconic(w.Handle),"transfer does not reveal minimized window",results);Assert(!Native.OnSecondaryMonitor(w.Handle),"minimized restore position now on primary",results);
            Assert(WindowMover.Move(w,true,out error),"move then reveal: "+error,results);for(int i=0;i<30&&Native.IsIconic(w.Handle);i++){Application.DoEvents();Thread.Sleep(10);}
            Assert(!Native.IsIconic(w.Handle)&&Native.OnSecondaryMonitor(w.Handle),"restored only after target monitor set",results);
            Native.ShowWindowAsync(w.Handle,3);Application.DoEvents();Thread.Sleep(100);Application.DoEvents();
            Assert(WindowMover.Move(w,false,out error),"maximized transfer: "+error,results);Assert(!Native.OnSecondaryMonitor(w.Handle),"maximized window moved to primary",results);
            fixture.Close();
        }
    }
    public void RunNativeToggleTest(List<string> results){
        IntPtr previous=Native.GetForegroundWindow();
        using(var fixture=new Form{Text="WindowShelf 临时最小化验证",Size=new Size(280,100),StartPosition=FormStartPosition.CenterScreen,ShowInTaskbar=false}){
            fixture.Show();Application.DoEvents();Native.SetForegroundWindow(fixture.Handle);Application.DoEvents();
            Assert(Native.GetAncestor(Native.GetForegroundWindow(),3)==fixture.Handle,"native fixture is foreground",results);
            var w=new WindowItem{Handle=fixture.Handle,Pid=Process.GetCurrentProcess().Id};
            ActivateWindow(w);for(int i=0;i<40&&!Native.IsIconic(w.Handle);i++){Application.DoEvents();Thread.Sleep(10);}
            Assert(Native.IsIconic(w.Handle),"actual foreground window minimized",results);
            ActivateWindow(w);for(int i=0;i<40&&Native.IsIconic(w.Handle);i++){Application.DoEvents();Thread.Sleep(10);}
            Assert(!Native.IsIconic(w.Handle),"actual minimized window restored",results);
            for(int i=0;i<50&&Native.GetAncestor(Native.GetForegroundWindow(),3)!=w.Handle;i++){Application.DoEvents();Thread.Sleep(10);}
            Assert(Native.GetAncestor(Native.GetForegroundWindow(),3)==w.Handle,"restored window returned to foreground",results);
            fixture.Close();
        }
        if(previous!=IntPtr.Zero&&Native.IsWindow(previous))Native.SetForegroundWindow(previous);
    }
    static void Assert(bool value,string test,List<string> results){if(!value)throw new Exception("FAIL: "+test);results.Add("PASS: "+test);}
}
static class Program {
    public static bool UiVerification;
    [STAThread] static void Main(string[] args){
        try{Native.SetProcessDpiAwarenessContext(new IntPtr(-4));}catch{}
        Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
        UiVerification=args.Length>0&&args[0]=="--ui-verify";
        string data=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"WindowShelf");
        string overridePath=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"window-shelf.data-dir");
        if(File.Exists(overridePath))data=Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,File.ReadAllText(overridePath).Trim()));
        int dataArg=Array.IndexOf(args,"--data-dir");if(dataArg>=0&&dataArg+1<args.Length)data=Path.GetFullPath(args[dataArg+1]);
        if(args.Length>0&&args[0]=="--selftest"){
            string path=args.Length>1?args[1]:Path.Combine(data,"selftest.json");Directory.CreateDirectory(Path.GetDirectoryName(path));var results=new List<string>();
            try{using(var f=new ShelfForm(Path.Combine(Path.GetDirectoryName(path),"test-state"),true)){f.RunUiTests(results);if(args.Contains("--native-test"))f.RunNativeToggleTest(results);if(args.Contains("--monitor-test"))f.RunMonitorTests(results);using(var bmp=new Bitmap(f.Width,f.Height)){f.DrawToBitmap(bmp,new Rectangle(0,0,f.Width,f.Height));bmp.Save(Path.ChangeExtension(path,"png"));}}File.WriteAllText(path,new JavaScriptSerializer().Serialize(new{passed=true,tests=results}));}
            catch(Exception e){File.WriteAllText(path,new JavaScriptSerializer().Serialize(new{passed=false,tests=results,error=e.ToString()}));Environment.ExitCode=1;}return;
        }
        if(args.Length>0&&args[0]=="--fixture"){
            var a=new Form{Text="WindowShelf 验证窗口 A",Width=420,Height=160,StartPosition=FormStartPosition.Manual,Location=new Point(500,300)};
            var b=new Form{Text="WindowShelf 验证窗口 B",Width=420,Height=160,StartPosition=FormStartPosition.Manual,Location=new Point(550,480)};
            a.Controls.Add(new Label{Text="仅用于原型验证，可以安全关闭。",Dock=DockStyle.Fill});b.Controls.Add(new Label{Text="第二个测试窗口；关闭不影响真实应用。",Dock=DockStyle.Fill});
            var timer=new System.Windows.Forms.Timer{Interval=2500};timer.Tick+=delegate{if(!a.IsDisposed)a.Text="WindowShelf 验证窗口 A · 标题已更新";timer.Stop();};
            int count=2;FormClosedEventHandler closed=delegate{if(--count==0)Application.ExitThread();};a.FormClosed+=closed;b.FormClosed+=closed;a.Show();b.Show();timer.Start();Application.Run();return;
        }
        bool created;using(var mutex=new Mutex(true,"Local\\WindowShelfPrototype",out created)){if(!created)return;Application.Run(new ShelfForm(data,false));}
    }
}
}
