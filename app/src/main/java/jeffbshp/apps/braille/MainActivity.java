package jeffbshp.apps.braille;

import android.content.Intent;
import android.graphics.Typeface;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.support.v7.widget.Toolbar;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.TextView;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.regex.Pattern;

public class MainActivity extends AppCompatActivity implements View.OnClickListener {

    private static final int NUM_CELLS = 4;
    private static final Pattern digits = Pattern.compile("^ |[0-9]");
    private TextView editText;
    private BrailleCell cells[];
    private ListView listResults;
    private ArrayList<BrailleContraction> data;

    @Override
    protected void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        Toolbar toolbar = (Toolbar) findViewById(R.id.app_toolbar);
        setSupportActionBar(toolbar);
        Button buttonBackspace = (Button) findViewById(R.id.button_backspace);
        Button buttonClear = (Button) findViewById(R.id.button_clear);
        buttonBackspace.setOnClickListener(this);
        buttonClear.setOnClickListener(this);

        loadData();

        int ids[] = new int[2];
        ids[0] = R.id.result_long;
        ids[1] = R.id.result_short;
        final MultiColumnAdapter adapt = new MultiColumnAdapter(this, R.layout.search_result, ids, search(" "));
        listResults = (ListView) findViewById(R.id.list_results);
        listResults.setAdapter(adapt);
        listResults.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                hideKeyboard(v);
                return false;
            }
        });

        cells = new BrailleCell[NUM_CELLS];
        LinearLayout cellLayout = (LinearLayout) findViewById(R.id.cells);
        for (int i = 0; i < NUM_CELLS; i++) {
            cells[i] = new BrailleCell(this);
            cells[i].setOnClickListener(this);
            cellLayout.addView(cells[i]);
        }

        editText = (TextView) findViewById(R.id.edit_text);
        editText.setOnFocusChangeListener(new View.OnFocusChangeListener() {
            @Override
            public void onFocusChange(View v, boolean hasFocus) {
                if (!hasFocus) { hideKeyboard(v); }
            }
        });
        editText.addTextChangedListener(new TextWatcher() {
            @Override
            public void beforeTextChanged(CharSequence s, int start, int count, int after) {}
            @Override
            public void onTextChanged(CharSequence s, int start, int before, int count) {}
            @Override
            public void afterTextChanged(Editable s) {
                String text = s.toString();
                ArrayList<CharSequence[]> results;
                if (digits.matcher(text).find()) {
                    String states[] = text.split(" ");
                    StringBuilder query = new StringBuilder();
                    for (int i = 0; i < NUM_CELLS; i++) {
                        if (i < states.length) { cells[i].setState(states[i]); }
                        else { cells[i].setState(""); }
                        char unicode = cells[i].getUnicode();
                        if (unicode != '\u2800') { query.append(unicode); }
                    }
                    results = search(query.toString());
                } else {
                    results = search(text);
                }
                adapt.setList(results);
                adapt.notifyDataSetChanged();
                listResults.setSelectionAfterHeaderView();
            }
        });
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        MenuInflater inflater = getMenuInflater();
        inflater.inflate(R.menu.actionbar_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.action_info:
                Intent intent = new Intent(this, InfoActivity.class);
                startActivity(intent);
                return true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    private void backspace() {
        String text = editText.getText().toString();
        if (digits.matcher(text).find()) {
            for (int i = NUM_CELLS - 1; i >= 0; i--) {
                if (!cells[i].getState().isEmpty()) {
                    cells[i].setState("");
                    break;
                }
            }
            updateText();
        } else {
            int length = text.length();
            int end = length > 0 ? length - 1 : 0;
            editText.setText(text.substring(0, end));
        }
    }

    private void clearCells() {
        for (BrailleCell cell : cells) {
            cell.setState("");
        }
        updateText();
    }

    private void updateText() {
        StringBuilder sb = new StringBuilder();
        boolean empty = true;
        for (int i = NUM_CELLS - 1; i >= 0; i--) {
            String state = cells[i].getState();
            if (!empty) {
                sb.insert(0, " " + state);
            } else if (!state.isEmpty()) {
                sb.insert(0, " " + state);
                empty = false;
            }
        }
        if (sb.length() > 0) {
            sb.deleteCharAt(0);
        }
        editText.setText(sb.toString());
    }

    @Override
    public void onClick(View v) {
        hideKeyboard(v);
        switch (v.getId()) {
            case R.id.button_backspace:
                backspace();
                break;
            case R.id.button_clear:
                clearCells();
                break;
            case R.id.dot0: case R.id.dot1: case R.id.dot2: case R.id.dot3: case R.id.dot4: case R.id.dot5:
                updateText();
                break;
            default:
                break;
        }
    }

    private void hideKeyboard(View view) {
        InputMethodManager imm = (InputMethodManager) getSystemService(INPUT_METHOD_SERVICE);
        imm.hideSoftInputFromWindow(view.getWindowToken(), 0);
    }

    private ArrayList<CharSequence[]> search(String query) {
        ArrayList<CharSequence[]> results = new ArrayList<>();
        for (BrailleContraction contraction : data) {
            String l = contraction.longForm.toString();
            String s = contraction.shortForm.toString();
            String b = contraction.braille;
            if (query.startsWith(" ") || l.contains(query) || s.startsWith(query) || b.startsWith(query)) {
                results.add(contraction.toSpannableArray());
            }
        }
        return results;
    }

    private void loadData() {

        BrailleContraction.setTypeface(Typeface.createFromAsset(getAssets(), "fonts/JBraille/JBraille.ttf"));
        ArrayList<BrailleContraction> data = new ArrayList<>();
        InputStream in = getResources().openRawResource(R.raw.data);
        BufferedReader reader = new BufferedReader(new InputStreamReader(in));

        try {
            int lineNumber = 0;
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
                if (lineNumber <= 25) {
                    BrailleContraction.addCharacter(longForm.charAt(0), shortForm.charAt(0));
                }
                data.add(new BrailleContraction(longForm.toString(), shortForm));
                line = reader.readLine();
                lineNumber++;
            }
        } catch (IOException e) {
            e.printStackTrace();
        }

        this.data = data;
    }
}
