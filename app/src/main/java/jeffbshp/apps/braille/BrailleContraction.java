package jeffbshp.apps.braille;

import android.graphics.Typeface;
import android.text.Spannable;
import android.text.SpannableStringBuilder;

import java.util.HashMap;
import java.util.Map;

public class BrailleContraction {

    private static Map<Character, Character> characters = new HashMap<>();
    private static Typeface typeface;

    public final SpannableStringBuilder longForm;
    public final SpannableStringBuilder shortForm;
    public final String braille;

    public BrailleContraction(String longForm, String shortForm) {
        this.longForm = new SpannableStringBuilder(longForm);
        this.shortForm = new SpannableStringBuilder(shortForm);
        this.braille = convertToBraille(shortForm);
        setSpans();
    }

    private void setSpans() {
        int spanStart = -1;
        int length = shortForm.length();
        for (int i = 0; i < length; i++) {
            final char c = shortForm.charAt(i);
            final boolean isBraille = c >= 0x2800 && c <= 0x283f;
            if (spanStart < 0 && isBraille) {
                spanStart = i;
            }
            if (spanStart >= 0 && !isBraille) {
                shortForm.setSpan(new CustomTypefaceSpan(typeface), spanStart, i, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                spanStart = -1;
            } else if (spanStart >= 0 && i == length - 1) {
                shortForm.setSpan(new CustomTypefaceSpan(typeface), spanStart, i + 1, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                spanStart = -1;
            }
        }
    }

    public Spannable[] toSpannableArray() {
        Spannable[] array = new Spannable[2];
        array[0] = longForm;
        array[1] = shortForm;
        return array;
    }

    public static void addCharacter(char c, char b) {
        characters.put(c, b);
    }

    public static void setTypeface(Typeface typeface) {
        BrailleContraction.typeface = typeface;
    }

    private static String convertToBraille(String s) {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < s.length(); i++) {
            char c = s.charAt(i);
            if (c >= 0x2800 && c <= 0x283f) {
                sb.append(c);
            } else if (characters.containsKey(c)) {
                sb.append(characters.get(c));
            }
        }
        return sb.toString();
    }
}
