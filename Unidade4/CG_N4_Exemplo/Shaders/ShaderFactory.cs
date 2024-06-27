using CG_Biblioteca;
using System;

namespace gcgcg.Shaders;

public static class ShaderFactory
{
    public static Shader CreateShader(ShaderType shaderType)
    {
        string vertexShaderPath = shaderType switch
        {
            ShaderType.Branca or ShaderType.Vermelha or ShaderType.Verde or ShaderType.Azul or ShaderType.Ciano or ShaderType.Magenta or ShaderType.Amarela => "Shaders/Colors/shader.vert",
            ShaderType.Textura => "Shaders/Texture/shader.vert",
            ShaderType.BasicLighting => "Shaders/BasicLighting/shader.vert",
            ShaderType.DirectionalLights => "Shaders/DirectionalLights/shader.vert",
            ShaderType.LightingMaps => "Shaders/LightingMaps/shader.vert",
            ShaderType.MultipleLights => "Shaders/MultipleLights/shader.vert",
            ShaderType.PointLights => "Shaders/PointLights/shader.vert",
            ShaderType.Spotlight => "Shaders/Spotlight/shader.vert",
            _ => throw new ArgumentOutOfRangeException(nameof(shaderType), shaderType, null)
        };

        string fragmentShaderPath = shaderType switch
        {
            ShaderType.Branca => "Shaders/Colors/shaderBranca.frag",
            ShaderType.Vermelha => "Shaders/Colors/shaderVermelha.frag",
            ShaderType.Verde => "Shaders/Colors/shaderVerde.frag",
            ShaderType.Azul => "Shaders/Colors/shaderAzul.frag",
            ShaderType.Ciano => "Shaders/Colors/shaderCiano.frag",
            ShaderType.Magenta => "Shaders/Colors/shaderMagenta.frag",
            ShaderType.Amarela => "Shaders/Colors/shaderAmarela.frag",
            ShaderType.Textura => "Shaders/Texture/shader.frag",
            ShaderType.BasicLighting => "Shaders/BasicLighting/lighting.frag",
            ShaderType.DirectionalLights => "Shaders/DirectionalLights/lighting.frag",
            ShaderType.LightingMaps => "Shaders/LightingMaps/lighting.frag",
            ShaderType.MultipleLights => "Shaders/MultipleLights/lighting.frag",
            ShaderType.PointLights => "Shaders/PointLights/lighting.frag",
            ShaderType.Spotlight => "Shaders/Spotlight/lighting.frag",
            _ => throw new ArgumentOutOfRangeException(nameof(shaderType), shaderType, null)
        };

        return new Shader(vertexShaderPath, fragmentShaderPath);
    }
}
