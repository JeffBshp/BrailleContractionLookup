package jeffbshp.apps.braille;

import android.graphics.Typeface;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.text.Editable;
import android.text.Spannable;
import android.text.TextWatcher;
import android.view.MotionEvent;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.TextView;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.List;
import java.util.regex.Pattern;

public class MainActivity extends AppCompatActivity implements View.OnClickListener {

    private static final int MAX_CELLS = 4;
    private static final Pattern digits = Pattern.compile("^ |[^0-9 ]");
    private TextView editText;
    private LinearLayout cellLayout;
    private BrailleCell cells[];
    private Button buttonMinus;
    private Button buttonPlus;
    private ListView listResults;
    private List<BrailleContraction> data;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        buttonMinus = (Button) findViewById(R.id.button_minus);
        buttonMinus.setOnClickListener(this);
        buttonPlus = (Button) findViewById(R.id.button_plus);
        buttonPlus.setOnClickListener(this);

        BrailleContraction.setTypeface(Typeface.createFromAsset(getAssets(), "fonts/Swell-Braille.ttf"));
        loadData();
        final ArrayAdapter<Spannable> adapter = new ArrayAdapter<Spannable>(this, R.layout.search_result, R.id.result_text, new ArrayList<Spannable>());
        listResults = (ListView) findViewById(R.id.list_results);
        listResults.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                hideKeyboard(v);
                return false;
            }
        });
        listResults.setAdapter(adapter);
        adapter.addAll(search(" "));

        cells = new BrailleCell[MAX_CELLS];
        cellLayout = (LinearLayout) findViewById(R.id.cells);
        for (int i = 0; i < MAX_CELLS; i++) {
            cells[i] = new BrailleCell(this);
            cells[i].setOnClickListener(this);
        }
        cellLayout.addView(cells[0]);

        editText = (TextView) findViewById(R.id.edit_text);
        editText.setOnFocusChangeListener(new View.OnFocusChangeListener() {
            @Override
            public void onFocusChange(View v, boolean hasFocus) {
                if (!hasFocus) {
                    hideKeyboard(v);
                }
            }
        });
        editText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}
            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {}
            @Override
            public void afterTextChanged(Editable s) {
                String string = s.toString();
                List<Spannable> results = null;
                if (digits.matcher(string).find()) {
                    results = search(string);
                } else {
                    String states[] = string.split(" ");
                    while (cellLayout.getChildCount() < states.length) {
                        addCell();
                        if (cellLayout.getChildCount() == cells.length) break;
                    }
                    StringBuilder query = new StringBuilder();
                    for (int i = 0; i < cellLayout.getChildCount(); i++) {
                        if (i < states.length) cells[i].setState(states[i]);
                        else cells[i].setState("");
                        char unicode = cells[i].getUnicode();
                        if (unicode != '\u2800') query.append(unicode);
                    }
                    results = search(query.toString());
                }
                adapter.clear();
                adapter.addAll(results);
                adapter.notifyDataSetChanged();
            }
        });
    }

    private void addCell() {
        int count = cellLayout.getChildCount();
        if (count < cells.length) {
            cellLayout.addView(cells[count]);
        }
    }

    private void removeCell() {
        int count = cellLayout.getChildCount();
        cells[count - 1].setState("");
        if (count > 1) {
            cellLayout.removeViewAt(count - 1);
        }
    }

    private void updateText() {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < cellLayout.getChildCount(); i++) {
            String state = cells[i].getState();
            if (!state.isEmpty()) {
                sb.append(state);
            }
            sb.append(" ");
        }
        sb.deleteCharAt(sb.length() - 1);
        editText.setText(sb.toString());
    }

    @Override
    public void onClick(View v) {
        hideKeyboard(v);
        if (v.getId() == R.id.button_minus) {
            removeCell();
        } else if (v.getId() == R.id.button_plus) {
            addCell();
        }
        updateText();
    }

    private void hideKeyboard(View view) {
        InputMethodManager imm = (InputMethodManager) getSystemService(INPUT_METHOD_SERVICE);
        imm.hideSoftInputFromWindow(view.getWindowToken(), 0);
    }

    private List<Spannable> search(String query) {
        List<Spannable> results = new ArrayList<Spannable>();
        for (BrailleContraction b : data) {
            if (query.startsWith(" ") || b.longForm.toString().contains(query) || b.shortForm.toString().startsWith(query) || b.braille.startsWith(query)) {
                results.add(b.toSpannable());
            }
        }
        return results;
    }

    private void loadData() {

        List<BrailleContraction> data = new ArrayList<BrailleContraction>();
        InputStream in = getResources().openRawResource(R.raw.data);
        BufferedReader reader = new BufferedReader(new InputStreamReader(in));

        try {
            String line = reader.readLine();
            while (line != null) {
                String split[] = line.split(" ");
                StringBuilder longForm = new StringBuilder(split[0]);
                String shortForm = split[split.length - 1];
                if (split.length > 2) {
                    for (int i = 1; i < split.length - 1; i++) {
                        longForm.append(" ").append(split[i]);
                    }
                }
                if (longForm.length() == 1 && shortForm.length() == 1) {
                    BrailleContraction.addCharacter(longForm.charAt(0), shortForm.charAt(0));
                }
                data.add(new BrailleContraction(longForm.toString(), shortForm));
                line = reader.readLine();
            }
        } catch (IOException e) {
            e.printStackTrace();
        }

        this.data = data;
    }
}
