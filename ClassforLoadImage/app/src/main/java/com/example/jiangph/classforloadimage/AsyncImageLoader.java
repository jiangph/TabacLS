package com.example.jiangph.classforloadimage;

import android.graphics.drawable.Drawable;

import java.lang.ref.SoftReference;
import java.net.URL;
import java.util.HashMap;
import java.util.Map;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.logging.Handler;

/**
 * Created by jiangph on 15-12-2.
 * Handler+ExecutorService(线程池)+MessageQuene+缓存模式
 * 将整个代码封装在一个类中，为避免多次下载同一副图的问题，使用了本地缓存
 */
public class AsyncImageLoader {
    //为加快速度，在内存中开启缓存（主要应用于重复图片较多时，或者同一个图片要多次被访问，比如在listView时来回滚动）
    public Map<String,SoftReference<Drawable>>imageCache=new HashMap<String,SoftReference<Drawable>>();

    private ExecutorService executorService= Executors.newFixedThreadPool(5); //固定五个线程来执行人物
    private final android.os.Handler handler=new android.os.Handler();

    /***
     *
     * @param imageUrl  图像url地址
     * @param callback  回调借口
     * @return
     */

    public Drawable loadDrawable(final String imageUrl,final ImageCallback callback)
    {
        //如果缓存过就从缓存中取出数据
        if(imageCache.containsKey(imageUrl)){
            SoftReference<Drawable> softReference=imageCache.get(imageUrl);
            if (softReference.get()!=null)
            {
                return softReference.get();
            }
        }
        //缓存中没有图像，则从网络上取出数据，并将取出的数据存到内存中
        executorService.submit(new Runnable() {
            @Override
            public void run() {
                try{
                    final Drawable drawable=loadImageFromUrl(imageUrl);
                    //将此图片放入到缓存当中
                    imageCache.put(imageUrl,new SoftReference<Drawable>(drawable));
                    handler.post(new Runnable() {
                        @Override
                        public void run() {
                            callback.imageLoaded(drawable);
                        }
                    });
                }
                catch (Exception e)
                {
                    e.printStackTrace();
                }
            }
        });
        return null;
    }

    /***
     * 从网络上取数据方法
     * @param imageUrl 图片链接
     * @return
     */
    protected Drawable loadImageFromUrl(String imageUrl)
    {
        try{
            return Drawable.createFromStream(new URL(imageUrl).openStream(),"image.png");
        }catch (Exception e)
        {
            throw new RuntimeException(e);
        }
    }









    //对外界开放的回接口
    public interface ImageCallback
    {
        //注意 此方法是用来设置目标图像的图像资源
        public void imageLoaded(Drawable imageDrawable);
    }












}
