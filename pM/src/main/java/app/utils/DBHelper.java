package app.utils;

import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

import app.Entity.State;

import static nl.qbusict.cupboard.CupboardFactory.cupboard;

/**
 * /**
 * Created by sweet on 15-10-4.
 */
public class DBHelper extends SQLiteOpenHelper {
    public DBHelper(Context context) {
        super(context, DBConstants.DB_NAME, null, DBConstants.DB_VERSION);
    }

    static {
        cupboard().register(State.class);
    }

    @Override
    public void onCreate(SQLiteDatabase db) {
        cupboard().withDatabase(db).createTables();
    }

    @Override
    public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        cupboard().withDatabase(db).upgradeTables();
    }

    @Override
    protected void finalize() throws Throwable {
        this.close();
        super.finalize();
    }

}
