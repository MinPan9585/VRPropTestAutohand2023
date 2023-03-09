Shader "Pico/URPCustomHand"
{
    Properties
    {
        [Space(10)]
        [HideInInspector]_MainTex ("MainTex", 2D) = "white" {}
        _MainColor("MainColor(a控制整体透明度)",Color) = (1,1,1,1)

        [Space(10)]
        _AlphaEnd("透明结束值",Range(0,3))= 0.1
        _AlphaSize("透明宽度",Range(0.0001,1)) = 0.1
        _AlphaPower("透明度强度",Range(1,255))=4

        [Space(10)]
        _SpecularColor("高光颜色",Color)=(1,1,1,1)
        _SpecularRange("高光范围",Range(0,10))=1
        _SpecularPower("高光强度",Range(0,10))=1


        [Space(10)]
        [HDR]_GlowColor("辉光颜色(a控制Glow显示的强度)",Color) = (1,1,1,1)
        _GlowRange("辉光范围",Range(0,5)) = 1

        [Space(10)]
        [HDR]_GlowColor1("辉光颜色1(a控制Glow显示的强度)",Color) = (1,1,1,1)
        _GlowRange1("辉光范围",Range(0,5)) = 1
        _StepValue("裁切值",Range(0,1))=0.1

        [Space(10)]
        [HDR]_GridColor("网格颜色(a控制网格显示强度)",Color) = (1,1,1,1)
        _GridCount("网格数量",Range(0,2000)) = 1
        _GridRange("网格范围",Range(0,20)) = 1

        [Space(10)]
        [HDR]_SpotLightColor("点亮颜色",Color) = (1,1,1,1)
        _ThumbPos("大拇指点亮中心",Vector) = (0,0,0,0)
        _ThumbRadius("大拇指点亮半径",Range(0,0.016)) = 0.01

        _IndexPos("食指点亮中心",Vector) = (0,0,0,0)
        _IndexRadius("食指点亮半径",Range(0,0.02)) = 0.01
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline" = "UniversalRenderPipeline"
        }
        Pass
        {
            Name "Depth"
            ZWrite On
            ColorMask 0
        }
        Pass
        {
            Name "ForwardBase"
            Tags
            {
                "LightMode"="UniversalForward"
            }
            LOD 100
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma target 3.0
            #include "CustomHandCommon.hlsl"
            ENDHLSL
        }
    }
}