package jeffbshp.apps.braille;

import android.graphics.Typeface;
import android.text.Spannable;
import android.text.SpannableStringBuilder;
import android.text.style.BackgroundColorSpan;

import java.util.HashMap;
import java.util.Map;

class BrailleContraction {

    private static Map<Character, Character> characters = new HashMap<>();
    private static Typeface typeface;

    final SpannableStringBuilder longForm;
    final SpannableStringBuilder shortForm;
    final String braille;

    BrailleContraction(String longForm, String shortForm) {
        this.longForm = new SpannableStringBuilder(longForm);
        this.shortForm = new SpannableStringBuilder(shortForm);
        this.braille = convertToBraille(shortForm);
        setSpans();
    }

    private void setSpans() {
        int spanStart = -1;
        for (int i = 0; i < this.shortForm.length(); i++) {
            final char c = this.shortForm.charAt(i);
            boolean isBraille = c >= 0x2800 && c <= 0x283f;
            if (spanStart < 0 && isBraille) {
                spanStart = i;
            }
            if (spanStart >= 0 && !isBraille) {
                shortForm.setSpan(new CustomTypefaceSpan(typeface), spanStart, i, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                shortForm.setSpan(new BackgroundColorSpan(0xFFEEEEEE), spanStart, i, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                spanStart = -1;
            } else if (spanStart >= 0 && i == this.shortForm.length() - 1) {
                shortForm.setSpan(new CustomTypefaceSpan(typeface), spanStart, i + 1, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                shortForm.setSpan(new BackgroundColorSpan(0xFFEEEEEE), spanStart, i + 1, Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
                spanStart = -1;
            }
        }
    }

    Spannable toSpannable() {
        return new SpannableStringBuilder(String.format("%-16s ", longForm)).append(shortForm);
    }

    static void addCharacter(char c, char b) {
        characters.put(c, b);
    }

    static void setTypeface(Typeface typeface) {
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
