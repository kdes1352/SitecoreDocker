using System;
using System.IO;
using System.Web;
using System.Web.UI;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.WebControls;

namespace AllinaHealth.Framework.Controls
{
    public class MvcEditFrame : IDisposable
    {
        #region Fields

        private readonly TextWriter _textWriter;
        private readonly EditFrame _editFrame;
        private bool _disposed;

        #endregion

        #region Constructor

        public MvcEditFrame(EditFrame editFrame, TextWriter textWriter)
        {
            _textWriter = textWriter;
            _editFrame = editFrame;
            if (_editFrame != null)
            {
                Render(_editFrame.RenderFirstPart);
            }
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            EndFrame();
        }

        public void EndFrame()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (_editFrame != null)
            {
                Render(_editFrame.RenderLastPart);
            }
        }

        private void Render(Action<HtmlTextWriter> renderer)
        {
            _textWriter.Write(RenderString(renderer).ToString());
        }

        public HtmlString GetHtmlFirstPart()
        {
            return RenderString(_editFrame.RenderFirstPart);
        }

        public HtmlString GetHtmlLastPart()
        {
            _disposed = true;
            return RenderString(_editFrame.RenderLastPart);
        }

        private HtmlString RenderString(Action<HtmlTextWriter> renderer)
        {
            try
            {
                using (var sw = new StringWriter())
                {
                    using (var htmlWriter = new HtmlTextWriter(sw))
                    {
                        renderer(htmlWriter);
                        return new HtmlString(sw.ToString());
                    }
                }
            }
            catch (Exception)
            {
                _disposed = true;
                var warningMessage = string.Format("MVC Edit Frame unable to render. Buttons: {0}, Datasource: {1}", _editFrame.Buttons, _editFrame.DataSource);
                Log.Warn(warningMessage, this);
            }

            return new HtmlString(string.Empty);
        }

        #endregion
    }
}