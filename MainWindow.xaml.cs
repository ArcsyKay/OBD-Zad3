using System.Windows;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using System.ComponentModel;
using System;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Text;
using System.Reflection;
using System.Collections;

namespace OBD3
{

    public partial class MainWindow : Window
    {
        private ObservableCollection<IStoredClass> clzList = new ObservableCollection<IStoredClass>();
        private ObservableCollection<Object> objList = new ObservableCollection<object>();

        private IObjectContainer db;
        private string File = null;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenDb4o_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if ((bool)dlg.ShowDialog())
            {
                if (db != null)
                {
                    db.Close();
                }

                File = dlg.FileName;
                tbFilePath.Text = File;
                try
                {
                    db = Db4oFactory.OpenFile(File);
                    var ext = db.Ext();
                    PopulateCombos(db);
                }
                catch(Exception)
                {
                    Msg(string.Format("Could not load database.\n"));
                }
                

                
            }
        }

        private void PopulateCombos(IObjectContainer db)
        {
            comboType.Items.Clear();
            clzList.Clear();
            comboIds.Items.Clear();
            var clzs = db.Ext().StoredClasses();
            foreach (var @class in clzs)
            {     
            if (!@class.GetName().Contains("Db4objects") && !@class.GetName().Contains("System"))
                {
                    comboType.Items.Add(@class.GetName() + " - " + @class.GetIDs().Length);
                    clzList.Add(@class);
                }
            }
            Msg(string.Format("{0} classes are listed.\n", comboType.Items.Count));
        }

        private void Msg(string v)
        {
            tbMsg.AppendText(v);
        }

        private void comboType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comboType.SelectedIndex < 0)
            {
                return;
            }

            comboIds.Items.Clear();
            objList.Clear();

            IStoredClass @class = clzList[comboType.SelectedIndex];
            var ids = @class.GetIDs();
            foreach (var id in ids)
            {
                var o = db.Ext().GetByID(id);
                objList.Add(o);
                comboIds.Items.Add(string.Format("ID: {0}", id));
            }
            Msg(string.Format("\nClass [{0}] fields:\n", @class.GetName()));
            foreach (var field in @class.GetStoredFields())
            {
                Msg(string.Format("{0} of type [{1}] \n", field.GetName(),field.GetStoredType()));
            }
            Msg(string.Format("\n{0} objects for class [{1}] are listed.\n", ids.Length, @class.GetName()));
        }

        private void comboIds_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var id = comboIds.SelectedIndex;
            if (id < 0)
            {
                return;
            }

            var @object = objList[id];
            var @class = clzList[comboType.SelectedIndex];

            Msg(string.Format("\n{0}=>\n", comboIds.SelectedValue));
            ListFields(@object, @class);
            Msg("\n===============END==============\n");
        }

        private void ListFields(object @object, IStoredClass @class, int depth = 0, string fieldName = "")
        {
            string pad = "";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < depth; i++)
            {
                sb.Append('\t');
            }
            pad = sb.ToString();

            // Nazwa pola
            if (depth > 0)
            {
                Msg(string.Format("{0}Field [{1}] of class [{2}] =>\n", pad, fieldName, @class.GetName()));
            }

            pad += "\t";

            // jeśli kolekcja to prześlij każdy element
            CollectionBase cs = @object as CollectionBase;
            if (cs != null)
            {
                IStoredClass classField1 = null;
                foreach (var object2 in cs)
                {
                    if (classField1 == null)
                    {
                        foreach (var classField in clzList)
                        {
                            if (classField.GetName() == object2.GetType().Name)
                            {
                                classField1 = classField;
                                break;
                            }
                        }
                    }

                    ListFields(object2, classField1, ++depth, fieldName + "[]");
                }

                return;
            }

            // prześlij każde pole jeśli to nie jest kolekcja
            foreach (var field in @class.GetStoredFields())
            {
                object value = null;
                try
                {
                    value = field.Get(@object);

                    if (field.GetStoredType().IsImmutable())
                    {
                        Msg(string.Format("{0}{1} of type[{2}] = {3}.\n", pad, field.GetName(), field.GetStoredType(), value));
                    }
                    else
                    {
                        IStoredClass classField1 = null;
                        foreach (var classField in clzList)
                        {
                            if (field.GetStoredType().GetName() == classField.GetName())
                            {
                                classField1 = classField;
                                break;
                            }
                        }
                        ListFields(value, classField1, ++depth, field.GetName());
                    }
                }
                catch (Exception ex)
                {
                    Msg(string.Format("Fail to load field [{0}] caused by {1}.\n",
                        field.GetName(), ex.Message));
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            tbMsg.Clear();
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(tbMsg.Text);
            tbMsg.Clear();
        }
        private void btnListAll_Click(object sender, RoutedEventArgs e)
        {
            IStoredClass @class = clzList[comboType.SelectedIndex];
            var ids = @class.GetIDs();
            Msg("\n===============LIST ALL==============\n");
            foreach (var id in ids)
            {
                var @object = db.Ext().GetByID(id);
                Msg(string.Format("\nID:{0} - {1} =>\n", id, @object));
                ListFields(@object, @class);
            }
        }

        private void btnListFilter_Click(object sender, RoutedEventArgs e)
        {
            IStoredClass @class = clzList[comboType.SelectedIndex];
            var ids = @class.GetIDs();
            Msg("\n===============LIST FILTERED==============\n");
            foreach (var id in ids)
            {
                var @object = db.Ext().GetByID(id);
                if (checkIfProperty(@object,@class))
                {                   
                    Msg(string.Format("\nID:{0} - {1} =>\n", id, @object));
                    ListFields(@object, @class);
                }
            }
        }
        private bool checkIfProperty(object @object,IStoredClass @class)
        {
            foreach (var field in @class.GetStoredFields())
            {
                if (field.GetName().ToString() == fieldTextBox.Text && field.Get(@object).ToString() == valueTextBox.Text)
                    return true;
                try
                   {
                      var value = field.Get(@object);
                    if (!field.GetStoredType().IsImmutable())
                    {
                        IStoredClass classField1 = null;
                        foreach (var classField in clzList)
                        {
                            if (field.GetStoredType().GetName() == classField.GetName())
                            {
                                classField1 = classField;
                                break;
                            }
                        }
                        return checkIfProperty(value, classField1);
                    }
                }
                catch { }
                    
            }
            return false;
        }
    }
}
#region Więcej detali
//Msg(string.Format("{0}Field [{1}] of class [{2}] =>\n", pad, fieldName, @class.GetName()));
//Msg(string.Format("{0}{1} of type[{2}] = {3}.\n", pad, field.GetName(), field.GetStoredType(), value));
//Msg(string.Format("Fail to load field [{0}] of object {1} of class [{2}] caused by {3}.",

#endregion