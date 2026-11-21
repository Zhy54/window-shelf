"""Generate fictitious documentation illustrations; never capture the desktop."""
from pathlib import Path
from PIL import Image, ImageDraw, ImageFont

ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/'docs/images'
OUT.mkdir(parents=True,exist_ok=True)
S=2
COL={'page':'#FFFFFF','bg':'#F3F4F6','head':'#E9ECF0','text':'#303844','edge':'#D8DDE4','active':'#E2E7ED','secondary':'#DCE5F2','accent':'#56759A','muted':'#66717E'}

def font(n):
    for p in (Path('C:/Windows/Fonts/msyh.ttc'),Path('/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf')):
        if p.exists():return ImageFont.truetype(str(p),n*S)
    return ImageFont.load_default()

def render(zh):
    im=Image.new('RGB',(960*S,610*S),COL['page']);d=ImageDraw.Draw(im)
    def box(x,y,w,h,c):d.rectangle((x*S,y*S,(x+w)*S,(y+h)*S),fill=c)
    def line(x1,y1,x2,y2,c,width=1):d.line((x1*S,y1*S,x2*S,y2*S),fill=c,width=width*S)
    def text(x,y,s,n=16,c=None):d.text((x*S,y*S),s,font=font(n),fill=c or COL['text'])
    def icon(x,y,letter):
        d.rounded_rectangle((x*S,y*S,(x+32)*S,(y+32)*S),radius=7*S,fill='#E0E7EF',outline=COL['accent'],width=S)
        bb=d.textbbox((0,0),letter,font=font(19));tw=(bb[2]-bb[0])/S
        text(x+(32-tw)/2,y+2,letter,19,COL['accent'])
    text(40,20,'Window Shelf',26)
    text(40,59,'中性灰＋雾蓝 · 配色示意' if zh else 'Neutral gray + mist blue · Illustrated preview',16,COL['muted'])
    text(56,100,'收起' if zh else 'Compact',14)
    text(218,100,'悬停展开' if zh else 'Hover to expand',14)
    panels=[(56,92,False),(218,280,True)]
    rows=[('F','文件管理' if zh else 'Files',False,False),('B','当前窗口' if zh else 'Active window',False,True),('W','Word',True,False)]
    for left,width,expanded in panels:
        top=132;height=410;box(left,top,width,height,COL['bg']);box(left,top,width,34,COL['head']);line(left+width,top,left+width,top+height,COL['edge'])
        text(left+10,top+7,'窗口 4' if expanded and zh else ('Windows 4' if expanded else '4'),14)
        if expanded:
            for dx in (0,5,10):d.ellipse(((left+width-26+dx)*S,(top+16)*S,(left+width-24+dx)*S,(top+18)*S),fill=COL['muted'])
        y=top+111
        for i,(letter,title,group,active) in enumerate(rows):
            yy=y+i*44
            if active:box(left+3,yy+1,width-7,42,COL['active']);box(left+2,yy+10,3,24,COL['accent'])
            if group:box(left+int(width/2),yy+1,int(width/2)-4,42,COL['secondary'])
            icon(left+30,yy+6,letter)
            if group:text(left+68,yy+12,'2',14)
            if expanded:
                if group:d.polygon([((left+89)*S,(yy+20)*S),((left+97)*S,(yy+20)*S),((left+93)*S,(yy+25)*S)],fill=COL['text'])
                text(left+100,yy+10,title,16)
                if not group:line(left+251,yy+15,left+265,yy+29,COL['muted'],2);line(left+265,yy+15,left+251,yy+29,COL['muted'],2)
        if expanded:
            for i,title in enumerate((('主屏文档' if zh else 'Main display'),('副屏文档' if zh else 'Other display'))):
                yy=y+(2+i)*44
                box(left+92,yy,width-93,44,COL['bg'])
                if i==1:box(left+92,yy+1,width-96,42,COL['secondary'])
                icon(left+104,yy+6,'W');text(left+141,yy+10,title,15)
                line(left+251,yy+15,left+265,yy+29,COL['muted'],2);line(left+265,yy+15,left+251,yy+29,COL['muted'],2)
    labels=[('bg','中性灰底色' if zh else 'Quiet neutral-gray surface'),('accent','当前窗口细标记' if zh else 'Slim active-window marker'),('secondary','副屏窗口雾蓝底色' if zh else 'Mist blue for the other display')]
    for i,(key,label) in enumerate(labels):
        yy=205+i*61;box(552,yy,24,24,COL[key]);text(592,yy,label,16)
    text(552,409,'固定图标位置' if zh else 'Fixed icon positions',17)
    text(552,442,'窗口标题与按钮向右展开' if zh else 'Titles and controls expand right',14,COL['muted'])
    text(40,573,'示意绘制 · 虚构窗口 · 非桌面截图' if zh else 'Illustration only · Fictitious windows · No desktop capture',13,COL['muted'])
    im.resize((960,610),Image.Resampling.LANCZOS).save(OUT/('preview-zh.png' if zh else 'preview-en.png'))

if __name__=='__main__':
    render(False);render(True)
    print('Rendered two illustrations without reading desktop contents.')
