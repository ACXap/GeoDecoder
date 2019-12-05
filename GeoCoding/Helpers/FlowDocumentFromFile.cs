// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;

namespace GeoCoding
{
    class FlowDocumentFromFile : Behavior<FlowDocumentScrollViewer>
    {
        public string FileName
        {
            get { return (string)GetValue(FileProperty); }
            set { SetValue(FileProperty, value); }
        }

        public static readonly DependencyProperty FileProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(FlowDocumentFromFile), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            SetDoc();
        }

        private void SetDoc()
        {
            FlowDocumentScrollViewer control = AssociatedObject;

            if (control == null)
            {
                throw new Exception("Чот упало в помощи");
            }

            Paragraph paragraph = new Paragraph();
            if(File.Exists(FileName))
            {
                paragraph.Inlines.Add(File.ReadAllText(FileName, Encoding.Default));
            }
            else
            {
                paragraph.Inlines.Add($"А нет файла помощи. Незачем было удалять файл: {FileName}");
            }
            
            control.Document = new FlowDocument(paragraph);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}