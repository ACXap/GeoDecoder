using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.IO;

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
            Control control = FindControl();

            if (control == null)
            {
                throw new Exception(
                    "Behavior needs to be attached to an Expander that is contained inside an ItemsControl");
            }

            Paragraph paragraph = new Paragraph();
            if(File.Exists(FileName))
            {
                paragraph.Inlines.Add(System.IO.File.ReadAllText(FileName, Encoding.Default));
            }
            else
            {
                paragraph.Inlines.Add($"А нет файла помощи. Незачем было удалять файл: {FileName}");
            }
            
            ((FlowDocumentScrollViewer)control).Document = new FlowDocument(paragraph);
        }

        private Control FindControl()
        {
            DependencyObject current = AssociatedObject;

            while (current != null && !(current is Control))
            {
                current = VisualTreeHelper.GetParent(current);
            }

            if (current == null)
            {
                return null;
            }

            return current as Control;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

    }
}