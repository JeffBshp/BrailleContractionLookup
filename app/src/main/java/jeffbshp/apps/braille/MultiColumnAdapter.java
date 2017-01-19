package jeffbshp.apps.braille;

import android.app.Activity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.TextView;

import java.util.ArrayList;

public class MultiColumnAdapter extends BaseAdapter {

    private LayoutInflater inflater;
    private int layoutId;
    private int columnIds[];
    private ArrayList<CharSequence[]> list;

    public MultiColumnAdapter(Activity activity, int layoutId, int columnIds[], ArrayList<CharSequence[]> list) {
        super();
        inflater = activity.getLayoutInflater();
        this.layoutId = layoutId;
        this.columnIds = columnIds;
        this.list = list;
    }

    public void setList(ArrayList<CharSequence[]> list) {
        this.list = list;
    }

    @Override
    public int getCount() {
        return list.size();
    }

    @Override
    public Object getItem(int position) {
        return list.get(position);
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    @Override
    public View getView(int position, View convertView, ViewGroup parent) {

        if(convertView == null){
            convertView = inflater.inflate(layoutId, parent, false);
        }

        CharSequence text[] = list.get(position);
        for (int i = 0; i < columnIds.length; i++) {
            TextView textView = (TextView) convertView.findViewById(columnIds[i]);
            textView.setText(text[i]);
        }

        return convertView;
    }
}
