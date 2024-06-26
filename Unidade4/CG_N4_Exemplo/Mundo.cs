#define CG_DEBUG
#define CG_Gizmo      
#define CG_OpenGL      
// #define CG_OpenTK
// #define CG_DirectX      
// #define CG_Privado      

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.IO;
using gcgcg.Shaders;

//FIXME: padrão Singleton

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        private static Objeto mundo = null;
        private char rotuloNovo = '?';
        private Objeto objetoSelecionado = null;

        private readonly float[] _sruEixos =
        {
          -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
           0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
           0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
        };

        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;

        private Shader _shaderBranca;
        private Shader _shaderVermelha;
        private Shader _shaderVerde;
        private Shader _shaderAzul;
        private Shader _shaderCiano;
        private Shader _shaderMagenta;
        private Shader _shaderAmarela;

        private Camera _camera;

        private Cubo _cuboMenor;
        private Cubo _cuboMaior;

        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
               : base(gameWindowSettings, nativeWindowSettings)
        {
            mundo ??= new Objeto(null, ref rotuloNovo); //padrão Singleton
        }


        const float raio = 10.0f;

        protected override void OnLoad()
        {
            base.OnLoad();

            Utilitario.Diretivas();
#if CG_DEBUG
            Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            GL.Enable(EnableCap.DepthTest);       // Ativar teste de profundidade
            GL.Enable(EnableCap.CullFace);     // Desenha os dois lados da face
                                               // GL.FrontFace(FrontFaceDirection.Cw);
                                               // GL.CullFace(CullFaceMode.FrontAndBack);

            #region Cores
            _shaderBranca = ShaderFactory.CreateShader(Shaders.ShaderType.Branca);
            _shaderVermelha = ShaderFactory.CreateShader(Shaders.ShaderType.Vermelha);
            _shaderVerde = ShaderFactory.CreateShader(Shaders.ShaderType.Verde);
            _shaderAzul = ShaderFactory.CreateShader(Shaders.ShaderType.Azul);
            _shaderCiano = ShaderFactory.CreateShader(Shaders.ShaderType.Ciano);
            _shaderMagenta = ShaderFactory.CreateShader(Shaders.ShaderType.Magenta);
            _shaderAmarela = ShaderFactory.CreateShader(Shaders.ShaderType.Amarela);
            #endregion

            #region Eixos: SRU  
            _vertexBufferObject_sruEixos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
            GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
            _vertexArrayObject_sruEixos = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            #endregion

            #region Objeto: ponto  
            objetoSelecionado = new Ponto(mundo, ref rotuloNovo, new Ponto4D(2.0, 0.0));
            objetoSelecionado.PrimitivaTipo = PrimitiveType.Points;
            objetoSelecionado.PrimitivaTamanho = 5;
            #endregion

            #region Objeto: Cubo
            _cuboMaior = new Cubo(mundo, ref rotuloNovo);
            _cuboMaior.MatrizEscalaXYZ(1, 1, 1);
            _cuboMaior._shaderObjeto = ShaderFactory.CreateShader(Shaders.ShaderType.Textura);
            _cuboMaior.texture = Texture.LoadFromFile(Path.Combine(Environment.CurrentDirectory, "Imagem", "grupo.png"));
            _cuboMaior.ObjetoAtualizar();

            objetoSelecionado = _cuboMaior;

            _cuboMenor = new Cubo(mundo, ref rotuloNovo);
            _cuboMenor.shaderCor = _shaderCiano;
            _cuboMenor.MatrizEscalaXYZ(0.3, 0.3, 0.3);
            _cuboMenor.MatrizTranslacaoXYZ(3, 0, 0);

            #endregion

            _camera = new Camera(new Vector3(0, 0, raio), 16.0f / 9.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            mundo.Desenhar(new Transformacao4D(), _camera);

#if CG_Gizmo
            Gizmo_Sru3D();
#endif
            SwapBuffers();
        }

        private Vector2 _lastMousePosition;
        private bool primeiroMovimento = true;
        private bool segurando = false;

        private float angulo = 0.0f;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _cuboMenor.TrocaEixoRotacao('z');
            _cuboMenor.MatrizRotacao(0.01);

            base.OnUpdateFrame(e);

            var mouse = MouseState;

            if (primeiroMovimento)
            {
                _lastMousePosition = new Vector2(mouse.X, mouse.Y);
                primeiroMovimento = false;
            }

            if (MouseState.IsButtonDown(MouseButton.Left) && objetoSelecionado != null)
            {
                if (!segurando)
                {
                    segurando = true;
                    _lastMousePosition = new Vector2(mouse.X, mouse.Y);
                }
                else
                {
                    var mouseOffset = new Vector2(mouse.X, mouse.Y) - _lastMousePosition;
                    _lastMousePosition = new Vector2(mouse.X, mouse.Y);

                    angulo += mouseOffset.X * 0.1f;

                    Ponto4D novoPonto = Matematica.GerarPtosCirculo(angulo, raio);
                    _camera.Position = new Vector3((float)novoPonto.X, 0, (float)novoPonto.Y);
                }
            }
            else
                segurando = false;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

#if CG_DEBUG
            Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);


            _camera.AspectRatio = Size.X / (float)Size.Y; ;
        }

        protected override void OnUnload()
        {
            mundo.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject_sruEixos);
            GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

            GL.DeleteProgram(_shaderBranca.Handle);
            GL.DeleteProgram(_shaderVermelha.Handle);
            GL.DeleteProgram(_shaderVerde.Handle);
            GL.DeleteProgram(_shaderAzul.Handle);
            GL.DeleteProgram(_shaderCiano.Handle);
            GL.DeleteProgram(_shaderMagenta.Handle);
            GL.DeleteProgram(_shaderAmarela.Handle);

            base.OnUnload();
        }

#if CG_Gizmo
        private void Gizmo_Sru3D()
        {
#if CG_OpenGL && !CG_DirectX
            var model = Matrix4.Identity;
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            // EixoX
            _shaderVermelha.SetMatrix4("model", model);
            _shaderVermelha.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderVermelha.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            // EixoY
            _shaderVerde.SetMatrix4("model", model);
            _shaderVerde.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderVerde.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);
            // EixoZ
            _shaderAzul.SetMatrix4("model", model);
            _shaderAzul.SetMatrix4("view", _camera.GetViewMatrix());
            _shaderAzul.SetMatrix4("projection", _camera.GetProjectionMatrix());
            _shaderAzul.Use();
            GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
        }
#endif

    }
}
