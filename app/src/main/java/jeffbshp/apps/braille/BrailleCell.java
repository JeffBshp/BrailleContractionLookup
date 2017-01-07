package jeffbshp.apps.braille;

import android.content.Context;
import android.util.AttributeSet;
import android.view.View;
import android.widget.CheckBox;
import android.widget.LinearLayout;

import com.jeffbshp.braille.R;

/**
 * Created by jeff on 5/16/16.
 */
public class BrailleCell extends LinearLayout implements View.OnClickListener {

    private CheckBox dots[];
    private byte state;

    public BrailleCell(Context context) {
        super(context);
        init(context);
    }

    public BrailleCell(Context context, AttributeSet attrs) {
        super(context, attrs);
        init(context);
    }

    public BrailleCell(Context context, AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
        init(context);
    }

    private void init(Context context) {
        inflate(context, R.layout.braille_cell, this);
        state = 0;
        dots = new CheckBox[6];
        dots[0] = (CheckBox) findViewById(R.id.dot0);
        dots[1] = (CheckBox) findViewById(R.id.dot1);
        dots[2] = (CheckBox) findViewById(R.id.dot2);
        dots[3] = (CheckBox) findViewById(R.id.dot3);
        dots[4] = (CheckBox) findViewById(R.id.dot4);
        dots[5] = (CheckBox) findViewById(R.id.dot5);
    }

    @Override
    public void setOnClickListener(OnClickListener l) {
        super.setOnClickListener(l);
        for (CheckBox dot : dots) {
            dot.setOnClickListener(l);
        }
    }

    @Override
    public void onClick(View v) {
        getState();
    }

    public String getState() {
        StringBuilder sb = new StringBuilder();
        byte state = 0;
        byte mask = 0x01;
        for (int i = 0; i < 6; i++) {
            CheckBox dot = dots[i];
            if (dot.isChecked()) {
                state |= mask;
                sb.append(i+1);
            }
            mask <<= 1;
        }
        this.state = state;
        return sb.toString();
    }

    public void setState(String newState) {
        byte mask = 0x01;
        byte bits = 0x3f;
        for (int i = 0; i < 6; i++) {
            boolean checked = newState.contains(String.valueOf(i+1));
            dots[i].setChecked(checked);
            if (checked) state |= mask & bits;
            else state &= mask ^ bits;
            mask <<= 1;
        }
    }

    public char getUnicode() {
        return (char) (0x2800 | state);
    }
}
