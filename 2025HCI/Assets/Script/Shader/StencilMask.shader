Shader "Custom/StencilMask"
{
    //遮罩区域，自身不渲染
    SubShader
    {
        Tags { "Queue"="Geometry-10" "RenderType"="Transparent" }

        // 不输出颜色，只写入 stencil
        ColorMask 0
        ZWrite Off
        ZTest Always

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass {}
    }
}
