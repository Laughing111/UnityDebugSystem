using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.UIWidgets.editor;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace RushDevelopement
{
    public class CustomizedConsole:UIWidgetsEditorWindow
    {
        public static ConsoleWindowState cwState;
        [MenuItem("RushDevelopment/CustomConsole")]
        private static void ShowCustomConsoleWindow()
        {
            CustomizedConsole cw= GetWindow<CustomizedConsole>("CustomConsole");
        }
        
        
        protected override Widget createWidget()
        {
            return new ConsoleWindow();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Debug.Log("aaa");
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            if (Event.current.keyCode==KeyCode.Space)
            {
                ConsoleWindowState.Ins.AddStr();
            }
        }
    }

    public class ConsoleWindow : StatefulWidget
    {   
        public override State createState()
        {
            return ConsoleWindowState.Ins;
        }
    }

    public class ConsoleWindowState : State<ConsoleWindow>
    {
        private static ConsoleWindowState ins;
       
        public static ConsoleWindowState Ins
        {
            get
            {
                if (ins == null)
                {
                    ins=new ConsoleWindowState();
                    
                }
                return ins;
            }
        }
        
        private string str = "A B C D E F G H I J K L M N";

        private string STR
        {
            set { str = value; }
            get { return str; }
        }

        public void AddStr()
        {
            //STR += " Q";
            Ins.setState(()=>str += " Q");
        }
        
        
        
        public override Widget build(BuildContext context)
        {
            return new MaterialApp(
                theme:new ThemeData(
                    appBarTheme:new AppBarTheme(color:Colors.cyan),
                    cardTheme:new CardTheme(color:Colors.white,elevation:2.0f)
                ),
                home:new Scaffold(
                    appBar:new AppBar(title:new Text("CustomConsole")),
                    body:new SingleChildScrollView(
                        padding:EdgeInsets.all(16.0f),
                        child:new Center(
                            child:new Column(
                                children:STR.Split(' ').Select(x=>new Text(x) as Widget).ToList()
                            )
                        ),
                        reverse:true
                    )   
                )
            );
        }
    }
}
