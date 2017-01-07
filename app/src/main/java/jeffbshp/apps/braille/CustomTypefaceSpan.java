package jeffbshp.apps.braille;

import android.graphics.Typeface;
import android.text.TextPaint;
import android.text.style.TypefaceSpan;

/**
 * Created by jeff on 5/17/16.
 */
public class CustomTypefaceSpan extends TypefaceSpan {

    private final Typeface typeface;

    public CustomTypefaceSpan(String family, Typeface typeface) {
        super(family);
        this.typeface = typeface;
    }

    @Override
    public void updateDrawState(TextPaint ds) {
        ds.setTypeface(typeface);
    }

    @Override
    public void updateMeasureState(TextPaint paint) {
        paint.setTypeface(typeface);
    }
}
