// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Numerics;
using LibreLancer.Infocards;
using LibreLancer;
using LibreLancer.ImUI;
using ImGuiNET;
namespace LancerEdit
{
    public class InfocardControl : IDisposable
    {
        BuiltRichText icard;
        MainWindow window;
        RenderTarget2D renderTarget;
        int renderWidth = -1, renderHeight = -1, rid = -1;
        public string InfocardText { get; private set; }
        public InfocardControl(MainWindow win, Infocard infocard, float initWidth)
        {
            window = win;
            icard = win.RichText.BuildText(infocard.Nodes, (int)initWidth, 0.7f);
        }
        public void SetInfocard(Infocard infocard)
        {
            icard.Dispose();
            InfocardText = infocard.ExtractText();
            icard = window.RichText.BuildText(infocard.Nodes, renderWidth > 0 ? renderWidth : 400, 0.7f);
        }
        public void Draw(float width)
        {
            icard.Recalculate(width);

            if (icard.Height != renderHeight || (int)width != renderWidth)
            {
                renderWidth = (int)width;
                renderHeight = (int)icard.Height;
                if (renderTarget != null)
                {
                    ImGuiHelper.DeregisterTexture(renderTarget.Texture);
                    renderTarget.Dispose();
                }
                renderTarget = new RenderTarget2D(renderWidth, renderHeight);
                rid = ImGuiHelper.RegisterTexture(renderTarget.Texture);
            }

            window.RenderState.RenderTarget = renderTarget;
            window.Viewport.Push(0, 0, renderWidth, renderHeight);
            var cc = window.RenderState.ClearColor;
            window.RenderState.ClearColor = Color4.Transparent;
            window.RenderState.ClearAll();
            window.RenderState.ClearColor = cc;
            window.Renderer2D.Start(renderWidth, renderHeight);
            window.RichText.RenderText(icard, 0, 0);
            window.Renderer2D.Finish();
            window.RenderState.RenderTarget = null;
            window.Viewport.Pop();

            var cPos = (Vector2)ImGui.GetCursorPos();
            var wPos = (Vector2)ImGui.GetWindowPos();
            var scrPos = -ImGui.GetScrollY();
            var mOffset = cPos + wPos + new Vector2(0, scrPos);
            var drawList = ImGui.GetWindowDrawList();
            drawList.AddImage((IntPtr)rid,
                new Vector2((int)mOffset.X, (int)mOffset.Y),
                new Vector2((int)(mOffset.X + renderWidth), (int)(mOffset.Y + icard.Height)),
                new Vector2(0, 1), new Vector2(1, 0));

            ImGui.InvisibleButton("##infocardbutton", new System.Numerics.Vector2(renderWidth, icard.Height));
        }
        public void Dispose()
        {
            renderTarget.Dispose();
        }
    }
}
