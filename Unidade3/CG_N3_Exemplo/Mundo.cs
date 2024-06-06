﻿#define CG_DEBUG
#define CG_Gizmo      
#define CG_OpenGL      
// #define CG_OpenTK
// #define CG_DirectX      
#define CG_Privado  

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        private static Objeto mundo = null;

        private char rotuloAtual = '?';
        private Objeto objetoSelecionado = null;

        private EditorVetorial editorVetorial;

        private readonly float[] _sruEixos =
        [
           0.0f,
            0.0f,
            0.0f, /* X- */
            0.5f,
            0.0f,
            0.0f, /* X+ */
            0.0f,
            0.0f,
            0.0f, /* Y- */
            0.0f,
            0.5f,
            0.0f, /* Y+ */
            0.0f,
            0.0f,
            0.0f, /* Z- */
            0.0f,
            0.0f,
            0.5f  /* Z+ */
        ];

        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;

        private int _vertexBufferObject_bbox;
        private int _vertexArrayObject_bbox;

        private Shader _shaderBranca;
        private Shader _shaderVermelha;
        private Shader _shaderVerde;
        private Shader _shaderAzul;
        private Shader _shaderCiano;
        private Shader _shaderMagenta;
        private Shader _shaderAmarela;

        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
          : base(gameWindowSettings, nativeWindowSettings)
        {
            mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
        }

        private Ponto4D GetPontoMouse()
        {
            int janelaLargura = ClientSize.X;
            int janelaAltura = ClientSize.Y;
            Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
            Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

            return sruPonto;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Utilitario.Diretivas();
#if CG_DEBUG
            Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            #region Cores
            _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
            _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
            _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
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

            editorVetorial = new EditorVetorial(mundo, ref rotuloAtual);

            objetoSelecionado = editorVetorial;

            List<Ponto4D> pontosPoligono = new List<Ponto4D>();
            Poligono poligono = new Poligono(mundo, ref rotuloAtual, pontosPoligono);
            editorVetorial.AdicionarPoligono(poligono);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            mundo.Desenhar(new Transformacao4D());

#if CG_Gizmo
            Gizmo_Sru3D();
            //Gizmo_BBox();
#endif
            if (editorVetorial.idxPoliogonoSelecionado != -1)
            {
                var transformada = new Transformacao4D();
                editorVetorial.poligonos[editorVetorial.idxPoliogonoSelecionado].Bbox().Desenhar(transformada);
            }

            SwapBuffers();
        }

        bool bEnterPressionado = false;
        bool boxSelecionado = false;
        

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
            #region Teclado
            var estadoTeclado = KeyboardState;
            if (estadoTeclado.IsKeyDown(Keys.Escape))
                Close();
            if (estadoTeclado.IsKeyPressed(Keys.Space))
            {
                if (objetoSelecionado == null)
                    objetoSelecionado = mundo;
                // objetoSelecionado.shaderObjeto = _shaderBranca;
                objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
                // objetoSelecionado.shaderObjeto = _shaderAmarela;
            }
            if (estadoTeclado.IsKeyPressed(Keys.D))
            {
                editorVetorial.RemoverPoligono();
            }
            if (estadoTeclado.IsKeyDown(Keys.V) && editorVetorial.idxPoliogonoSelecionado != -1)
            {
                editorVetorial.MoverVerticeMaisProximoPoligono(GetPontoMouse());
            }
            if (estadoTeclado.IsKeyPressed(Keys.E) && editorVetorial.idxPoliogonoSelecionado != -1)
            {
                if (editorVetorial.GetPoligonoAtualSelecionado().PontosListaTamanho == 2)
                { 
                    editorVetorial.RemoverPoligono();
                }
                else
                    editorVetorial.RemoverVerticeMaisProximoPoligono(GetPontoMouse());
            }
            if (estadoTeclado.IsKeyPressed(Keys.Enter))
            {
                editorVetorial.idxPoligonoAlterando = -1;
                editorVetorial.idxPoliogonoSelecionado = editorVetorial.poligonos.Count - 1;
            }
            if (estadoTeclado.IsKeyPressed(Keys.P))
            {
                editorVetorial.AlteraAberturaFechamentoPoligonoAtual();
            }
            if (estadoTeclado.IsKeyPressed(Keys.R) && editorVetorial.idxPoliogonoSelecionado != -1)
            {
                editorVetorial.GetPoligonoAtualSelecionado().ShaderObjeto = _shaderVermelha;
            }
            if (estadoTeclado.IsKeyPressed(Keys.G) && editorVetorial.idxPoliogonoSelecionado != -1)
            {
                editorVetorial.GetPoligonoAtualSelecionado().ShaderObjeto = _shaderVerde;
            }
            if (estadoTeclado.IsKeyPressed(Keys.B) && editorVetorial.idxPoliogonoSelecionado != -1)
            {
                editorVetorial.GetPoligonoAtualSelecionado().ShaderObjeto = _shaderAzul;
            }
            if (estadoTeclado.IsKeyPressed(Keys.Home) && editorVetorial.idxPoliogonoSelecionado != -1)
            {
                editorVetorial.GetPoligonoAtualSelecionado().MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
            }
            if (estadoTeclado.IsKeyPressed(Keys.End) && editorVetorial.idxPoliogonoSelecionado != -1)
            {
                editorVetorial.GetPoligonoAtualSelecionado().MatrizEscalaXYZBBox(2, 2, 2);
            }
            if (estadoTeclado.IsKeyPressed(Keys.D3) && editorVetorial.idxPoliogonoSelecionado != -1)
            {
                editorVetorial.GetPoligonoAtualSelecionado().MatrizRotacaoZBBox(10);
            }
            if (estadoTeclado.IsKeyPressed(Keys.D4) && editorVetorial.idxPoliogonoSelecionado != -1)
            {
                editorVetorial.GetPoligonoAtualSelecionado().MatrizRotacaoZBBox(-10);
            }
            if (estadoTeclado.IsKeyPressed(Keys.Left) && editorVetorial.idxPoliogonoSelecionado != -1)
                editorVetorial.GetPoligonoAtualSelecionado().MatrizTranslacaoXYZ(-0.05, 0, 0);
            if (estadoTeclado.IsKeyPressed(Keys.Right) && editorVetorial.idxPoliogonoSelecionado != -1)
                editorVetorial.GetPoligonoAtualSelecionado().MatrizTranslacaoXYZ(0.05, 0, 0);
            if (estadoTeclado.IsKeyPressed(Keys.Up) && editorVetorial.idxPoliogonoSelecionado != -1)
                editorVetorial.GetPoligonoAtualSelecionado().MatrizTranslacaoXYZ(0, 0.05, 0);
            if (estadoTeclado.IsKeyPressed(Keys.Down) && editorVetorial.idxPoliogonoSelecionado != -1)
                editorVetorial.GetPoligonoAtualSelecionado().MatrizTranslacaoXYZ(0, -0.05, 0);

            #endregion

            #region  Mouse

            if (MouseState.IsButtonPressed(MouseButton.Left))
                editorVetorial.SelecionaPoligono(GetPontoMouse());

            if (MouseState.IsButtonPressed(MouseButton.Right))
            {
                if (editorVetorial.idxPoligonoAlterando == -1)
                {
                    List<Ponto4D> pontosPoligono = new List<Ponto4D>();

                    char rotulo = '@';

                    Objeto obj = mundo.GrafocenaBusca(rotulo);
                    Objeto objAtual = mundo;

                    while (obj != null)
                    {
                        objAtual = obj;

                        rotulo = Utilitario.CharProximo(rotulo);
                        obj = mundo.GrafocenaBusca(rotulo);
                    }

                    Poligono poligono = new Poligono(objAtual, ref rotuloAtual, pontosPoligono);
                    editorVetorial.AdicionarPoligono(poligono);

                    editorVetorial.idxPoliogonoSelecionado = -1;
                }

                editorVetorial.AdicionarPontoPoligonoAtual(GetPontoMouse());
            }

            if (MouseState.IsButtonDown(MouseButton.Right) && objetoSelecionado != null)
            {
                
                if (editorVetorial.GetPoligonoAtualAlterando().PontosListaTamanho != 1)
                    editorVetorial.AlterarPontoPoligonoAtual(0, GetPontoMouse());
                else
                    editorVetorial.AdicionarPontoPoligonoAtual(GetPontoMouse());
            }
            #endregion

        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

#if CG_DEBUG
            Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload()
        {
            mundo.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject_sruEixos);
            GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

            GL.DeleteBuffer(_vertexBufferObject_bbox);
            GL.DeleteVertexArray(_vertexArrayObject_bbox);

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
            var transform = Matrix4.Identity;
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            // EixoX
            _shaderVermelha.SetMatrix4("transform", transform);
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
            // EixoY
            _shaderVerde.SetMatrix4("transform", transform);
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);
            // EixoZ
            _shaderAzul.SetMatrix4("transform", transform);
            _shaderAzul.Use();
            GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
        }
#endif

#if CG_Gizmo
        private void Gizmo_BBox()   //FIXME: não é atualizada com as transformações globais
        {
            if (objetoSelecionado != null)
            {

#if CG_OpenGL && !CG_DirectX

                float[] _bbox =
                {
        (float) objetoSelecionado.Bbox().ObterMenorX, (float) objetoSelecionado.Bbox().ObterMenorY, 0.0f, // A
        (float) objetoSelecionado.Bbox().ObterMaiorX, (float) objetoSelecionado.Bbox().ObterMenorY, 0.0f, // B
        (float) objetoSelecionado.Bbox().ObterMaiorX, (float) objetoSelecionado.Bbox().ObterMaiorY, 0.0f, // C
        (float) objetoSelecionado.Bbox().ObterMenorX, (float) objetoSelecionado.Bbox().ObterMaiorY, 0.0f  // D
      };

                _vertexBufferObject_bbox = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_bbox);
                GL.BufferData(BufferTarget.ArrayBuffer, _bbox.Length * sizeof(float), _bbox, BufferUsageHint.StaticDraw);
                _vertexArrayObject_bbox = GL.GenVertexArray();
                GL.BindVertexArray(_vertexArrayObject_bbox);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                var transform = Matrix4.Identity;
                GL.BindVertexArray(_vertexArrayObject_bbox);
                _shaderAmarela.SetMatrix4("transform", transform);
                _shaderAmarela.Use();
                GL.DrawArrays(PrimitiveType.LineLoop, 0, (_bbox.Length / 3));

#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
            }
        }
#endif

    }
}