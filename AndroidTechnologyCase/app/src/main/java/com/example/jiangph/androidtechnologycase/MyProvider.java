package com.example.jiangph.androidtechnologycase;

import android.content.ContentProvider;
import android.content.ContentValues;
import android.content.UriMatcher;
import android.database.Cursor;
import android.net.Uri;
import android.support.annotation.Nullable;
import android.util.Log;

/**
 * 使用contentProvider的时候,数据库不需要关闭
 * 我们在操作该contentprivider数据的时候，需要特定的uri去操作
 * Created by jiangph on 16-4-13.
 * uri的构造格式如下：content://<AndroidManifest.xml配置的provider的authorities>/自定义内容
 */
public class MyProvider extends ContentProvider {

    //声明一些变量
    private static int NOMATCH=-1;
    private static int INSERT=1;
    private static int QUERY=2;
    private static int UPDATE=3;
    private static int DELETE=4;

    //为了方便我们操作Google提供的UriMatcher，我们可以通过该类去高早我们能够匹配
    //的Uri,当然我们也可以自己进行匹配，但是那样容易书写错误
    private static UriMatcher matcher=new UriMatcher(NOMATCH);
    static {
        matcher.addURI("com.example.jiangph.androidtechnologycase.MyProvider","insert",INSERT);
        matcher.addURI("com.example.jiangph.androidtechnologycase.MyProvider","query",QUERY);
        matcher.addURI("com.example.jiangph.androidtechnologycase.MyProvider","update",UPDATE);
        matcher.addURI("com.example.jiangph.androidtechnologycase.MyProvider","delete",DELETE);
    }

    @Override
    public boolean onCreate() {
        Log.i("mxy","provider onCreate");
        return false;
    }

    @Nullable
    @Override
    public Cursor query(Uri uri, String[] projection, String selection, String[] selectionArgs, String sortOrder) {
        Log.i("mxy","provider query"+matcher.match(uri));
        if (QUERY==matcher.match(uri))
        {
            //在这里实现某个对象的sqllite操作
            //return cursor操作
        }
            return null;
    }

    @Nullable
    @Override
    public String getType(Uri uri) {
        Log.i("mxy","provider delete"+matcher.match(uri));
        return null;
    }

    @Nullable
    @Override
    public Uri insert(Uri uri, ContentValues values) {
        return null;
    }

    @Override
    public int delete(Uri uri, String selection, String[] selectionArgs) {
        return 0;
    }

    @Override
    public int update(Uri uri, ContentValues values, String selection, String[] selectionArgs) {
        return 0;
    }
}
