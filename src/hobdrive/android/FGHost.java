package hobdrive.android;

import android.graphics.*;
import android.graphics.Paint.FontMetricsInt;
import androidx.annotation.*;

import java.io.UnsupportedEncodingException;
import java.nio.*;

//@Keep
public class FGHost
{
	public final static int BSIZE = 20*1024;

    private static final byte OP_END = 0;
	private static final byte OP_DRAWSTRING = 1;
	private static final byte OP_DRAWRECT = 2;
	private static final byte OP_DRAWLINE = 3;
	private static final byte OP_DRAWELLIPSE = 4;
	private static final byte OP_FILLRECT = 5;
	private static final byte OP_FILLPOLY = 6;
	private static final byte OP_FILLELLIPSE = 7;
	private static final byte OP_DRAWSTRING2 = 8;
    private static final byte OP_SETFLAGS = 9;
    private static final byte OP_DRAWARC = 10;
	
    Canvas ACanvas;
    ByteBuffer b;
    Paint APaint = new Paint();
    int LineWidth = 1;
    int Flags = 0;

    public FGHost(Canvas canvas)
    {
    	ACanvas = canvas;
        b = ByteBuffer.allocateDirect(BSIZE);
        b.order(ByteOrder.nativeOrder());
    }
    
    public ByteBuffer getBuffer()
    {
    	return b;
    }
    
    byte[] sbuf = new byte[1024];
    int sbuf_len = 1024;
    
    String getString()
    {
    	int len = b.getInt() * 2;
        if (sbuf_len < len)
        {
            sbuf = new byte[len];
        }
    	b.get(sbuf, 0, len);
    	try {
			return new String(sbuf, 0, len, "UTF-16");
		} catch (UnsupportedEncodingException e) {
			return "Bad Encoding";
		}
    }


    public void flush()
    {
    	int x, y, x1, y1, w, h, x2, y2;
        float sa, swa;
        RectF r;
    	
        //android.util.Log.e("HOBD_FGHost", "flush");
    	b.rewind();
    	while(true)
    	{
	    	byte op = b.get();
            //android.util.Log.e("HOBD_FGHost", "op: "+op);
	    	switch(op)
	    	{
	    	case OP_END:
	    		return;
    		case OP_DRAWSTRING:
    			String s = getString();
                x = b.getInt();
                y = b.getInt();
    			APaint.setColor(b.getInt());
                APaint.setTextSize(b.getInt());
                //APaint.SetTypeface(Android.Graphics.Typeface.Default);//TODO
                APaint.setStyle(android.graphics.Paint.Style.FILL);
                APaint.setFlags(android.graphics.Paint.ANTI_ALIAS_FLAG);
                FontMetricsInt fm = APaint.getFontMetricsInt();
                int height = -fm.top;
                ACanvas.drawText(s, x, y+height, APaint);
    			break;
    		case OP_DRAWRECT:
                APaint.setColor(b.getInt());
                x1 = b.getInt();
                y1 = b.getInt();
                w = b.getInt();
                h = b.getInt();
                APaint.setFlags(0);
                APaint.setStyle(android.graphics.Paint.Style.STROKE);
                APaint.setStrokeWidth(LineWidth);
                ACanvas.drawRect(x1, y1, x1+w, y1+h, APaint);
                break;
            case OP_DRAWLINE:
                APaint.setColor(b.getInt());
                APaint.setStrokeWidth(b.getInt());
                x1 = b.getInt();
                y1 = b.getInt();
                x2 = b.getInt();
                y2 = b.getInt();
                APaint.setStyle(android.graphics.Paint.Style.STROKE);
                APaint.setFlags(0);
                ACanvas.drawLine(x1, y1, x2, y2, APaint);
                break;
            case OP_DRAWELLIPSE:
                APaint.setColor(b.getInt());
                x = b.getInt();
                y = b.getInt();
                w = b.getInt();
                h = b.getInt();
                APaint.setFlags(Flags);
                APaint.setStyle(android.graphics.Paint.Style.STROKE);
                APaint.setStrokeWidth(LineWidth);
                r = new android.graphics.RectF(x, y, x+w, y+h);
                ACanvas.drawOval(r, APaint);
                break;
            case OP_DRAWARC:
                APaint.setColor(b.getInt());
                x = b.getInt();
                y = b.getInt();
                w = b.getInt();
                h = b.getInt();
                sa = (float)b.getInt() / 1000;
                swa = (float)b.getInt() / 1000;
                APaint.setFlags(Flags);
                APaint.setStyle(android.graphics.Paint.Style.STROKE);
                APaint.setStrokeWidth(LineWidth);
                r = new android.graphics.RectF(x, y, x+w, y+h);
                ACanvas.drawArc(r, sa, swa, false, APaint);
                break;                
            case OP_FILLRECT:
                APaint.setColor(b.getInt());
                x1 = b.getInt();
                y1 = b.getInt();
                w = b.getInt();
                h = b.getInt();
                APaint.setFlags(Flags);
                APaint.setStyle(android.graphics.Paint.Style.FILL);
                APaint.setStrokeWidth(LineWidth);
                ACanvas.drawRect(x1, y1, x1+w, y1+h, APaint);
                break;
            case OP_FILLPOLY:
                APaint.setColor(b.getInt());
                APaint.setFlags(Flags);
                APaint.setStyle(android.graphics.Paint.Style.FILL);
                APaint.setStrokeWidth(LineWidth);
                Path p = new Path();
                int len = b.getInt();
                for (int i = 0; i < len; i++)
                {
                	x = b.getInt();
                	y = b.getInt();
                	if (i == 0)
                		p.moveTo(x, y);
                	else
                		p.lineTo(x, y);
                }
                ACanvas.drawPath(p, APaint);
                break;
            case OP_FILLELLIPSE:
                APaint.setColor(b.getInt());
                x = b.getInt();
                y = b.getInt();
                w = b.getInt();
                h = b.getInt();
                APaint.setFlags(Flags);
                APaint.setStyle(android.graphics.Paint.Style.FILL);
                APaint.setStrokeWidth(LineWidth);
                RectF r1 = new android.graphics.RectF(x, y, x+w, y+h);
                ACanvas.drawOval(r1, APaint);
                break;
            case OP_DRAWSTRING2:
    			String text = getString();
                x = b.getInt();
                y = b.getInt();
                w = b.getInt();
                h = b.getInt();
    			APaint.setColor(b.getInt());
                APaint.setTextSize(b.getInt());
                //APaint.SetTypeface(Android.Graphics.Typeface.Default);//TODO
                APaint.setStyle(android.graphics.Paint.Style.FILL);
                APaint.setFlags(android.graphics.Paint.ANTI_ALIAS_FLAG);
                
                fm = APaint.getFontMetricsInt();
                height = -fm.top;
                int cline = 0;
                int lineheight = -fm.top + fm.bottom;

                int coffset = 0;
                while(coffset < text.length())
                {
                    String tpart = text.substring(coffset);
                    int tlen = APaint.breakText(tpart, true, w, null);
                    int extralen = 0;
                    int spaceoffset = tpart.substring(tlen/2, tlen).lastIndexOf(" ");
                    if (spaceoffset <= 0)
                        spaceoffset = tpart.substring(tlen/2, tlen).lastIndexOf(".");
                    if (spaceoffset > 0 && coffset+tlen < text.length())
                    {
                        tlen = spaceoffset+tlen/2;
                        extralen = 1;
                    }
                    int croffset = tpart.indexOf("\n");
                    if (croffset >= 0 && croffset < tlen)
                    {
                        tlen = croffset;
                        extralen = 1;
                    }
                    ACanvas.drawText(tpart.substring(0,tlen), x, y + height + lineheight*cline, APaint);
                    coffset += tlen+extralen;
                    cline++;
                }
                break;
            case OP_SETFLAGS:
                Flags = b.getInt();
                APaint.setFlags(Flags);
                break;
            default:
    			android.util.Log.e("HOBD_FGHost", "Unknown command: "+op);
    			break;
	    	}
    	}
    }

}