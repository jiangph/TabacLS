package com.example.jiangph.classforloadimage;

import android.graphics.drawable.Drawable;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.widget.ImageView;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }


    private AsyncImageLoader asyncImageLoader=new AsyncImageLoader();

    //引入线程池，并引入内存缓存功能，并对外部调用了借口，简化调用过程
    private void loadImage4(final String url,final int id)
    {
        //如果缓存过就会从缓存中取出图像，ImageCallback接口中方法也不会被执行
        Drawable cacheImage=asyncImageLoader.loadDrawable(url,new AsyncImageLoader.ImageCallback()
        {
            public void imageLoaded(Drawable imageDrawable)
            {
                ((ImageView)findViewById(id)).setImageDrawable(imageDrawable);
            }
        });
        if (cacheImage!=null)
        {
            ((ImageView)findViewById(id)).setImageDrawable(cacheImage);
        }
    }







}
