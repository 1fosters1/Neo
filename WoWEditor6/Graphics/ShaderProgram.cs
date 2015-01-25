﻿using System;
using System.Linq;
using System.Text;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace WoWEditor6.Graphics
{
    class ShaderProgram
    {
        private static readonly ShaderResourceView[] gShaderViews = new ShaderResourceView[64];

        private VertexShader mVertexShader;
        private PixelShader mPixelShader;
        private readonly GxContext mContext;

        public ShaderBytecode VertexShaderCode { get; private set; }

        public ShaderProgram(GxContext context)
        {
            mContext = context;
        }

        public void SetVertexSampler(int slot, Sampler sampler)
        {
            mContext.Context.VertexShader.SetSampler(slot, sampler.Native);
        }

        public void SetPixelSampler(int slot, Sampler sampler)
        {
            mContext.Context.PixelShader.SetSampler(slot, sampler.Native);
        }

        public void SetVertexTexture(int slot, Texture texture)
        {
            mContext.Context.VertexShader.SetShaderResource(slot, texture.NativeView);
        }

        public void SetVertexTextures(int slot, params Texture[] textures)
        {
            for (var i = 0; i < textures.Length; ++i)
                gShaderViews[i] = textures[i].NativeView;

            mContext.Context.VertexShader.SetShaderResources(slot, textures.Length, gShaderViews);
        }

        public void SetVertexTexture(int slot, ShaderResourceView view)
        {
            mContext.Context.VertexShader.SetShaderResource(slot, view);
        }

        public void SetPixelTexture(int slot, Texture texture)
        {
            mContext.Context.PixelShader.SetShaderResource(slot, texture.NativeView);
        }

        public void SetPixelTexture(int slot, ShaderResourceView view)
        {
            mContext.Context.PixelShader.SetShaderResource(slot, view);
        }

        public void SetPixelTextures(int slot, params Texture[] textures)
        {
            for (var i = 0; i < textures.Length; ++i)
                gShaderViews[i] = textures[i].NativeView;

            mContext.Context.PixelShader.SetShaderResources(slot, textures.Length, gShaderViews);
        }

        public void SetPixelTextures(int slot, params ShaderResourceView[] textures)
        {
            mContext.Context.PixelShader.SetShaderResources(slot, textures);
        }

        public void SetVertexConstantBuffer(int slot, ConstantBuffer buffer)
        {
            mContext.Context.VertexShader.SetConstantBuffer(slot, buffer.Native);
        }

        public void SetVertexConstantBuffers(int slot, params ConstantBuffer[] buffers)
        {
            mContext.Context.VertexShader.SetConstantBuffers(slot, buffers.Select(b => b.Native).ToArray());
        }

        public void SetPixelConstantBuffer(int slot, ConstantBuffer buffer)
        {
            mContext.Context.PixelShader.SetConstantBuffer(slot, buffer.Native);
        }

        public void SetPixelConstantBuffers(int slot, params ConstantBuffer[] buffers)
        {
            mContext.Context.PixelShader.SetConstantBuffers(slot, buffers.Select(b => b.Native).ToArray());
        }

        public void SetVertexShader(string code, string entry)
        {
            var result = ShaderBytecode.Compile(Encoding.UTF8.GetBytes(code), entry, "vs_5_0", ShaderFlags.OptimizationLevel3);
            if (result.HasErrors)
                throw new ArgumentException(result.Message, "code");

            VertexShaderCode = result.Bytecode;
            mVertexShader = new VertexShader(mContext.Device, VertexShaderCode.Data);
        }

        public void SetPixelShader(string code, string entry)
        {
            using (var result = ShaderBytecode.Compile(Encoding.UTF8.GetBytes(code), entry, "ps_5_0",
                ShaderFlags.OptimizationLevel3))
            {
                if (result.HasErrors)
                    throw new ArgumentException(result.Message, "code");

                mPixelShader = new PixelShader(mContext.Device, result.Bytecode.Data);
            }
        }

        public void Bind()
        {
            mContext.Context.VertexShader.Set(mVertexShader);
            mContext.Context.PixelShader.Set(mPixelShader);
        }
    }
}
