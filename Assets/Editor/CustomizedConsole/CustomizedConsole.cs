using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.UIWidgets.editor;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEditor;
using UnityEngine;
using ScrollView = Unity.UIWidgets.widgets.ScrollView;

namespace RushDevelopement
{
    public class CustomizedConsole : UIWidgetsEditorWindow
    {
        [MenuItem("RushDevelopment/CustomConsole")]
        private static void ShowCustomConsoleWindow()
        {
            CustomizedConsole cw = GetWindow<CustomizedConsole>("CustomConsole");
        }

        protected override Widget createWidget()
        {
            return new ConsoleWindow(message);
        }


        protected override void OnEnable()
        {
            FontManager.instance.addFont(Resources.Load<Font>("MaterialIcons-Regular"), "Material Icons");
            base.OnEnable();

            Application.logMessageReceived += ReceiveDebugMsg;
        }

        public Notifier message = new Notifier();

        private void ReceiveDebugMsg(string condition, string stacktrace, LogType type)
        {
            DebugModel.Ins.ReceiveDebugMsg(condition, stacktrace, type);
            using (window.getScope())
            {
                message.SendMessage();
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            if (Event.current.keyCode == KeyCode.Space)
            {
                ReceiveDebugMsg(" Q", "UnityEngine.Debug", LogType.Log);
            }
        }
    }

    #region Debug信息数据类

    public class DebugModel
    {
        private static DebugModel ins;

        public static DebugModel Ins
        {
            get
            {
                if (ins == null)
                {
                    ins = new DebugModel();
                    ins.debugMsg = new List<string>();
                    ins.tacktraces = new List<string>();
                    ins.debugTypes = new List<string>();
                    ins.temp = "";
                }

                return ins;
            }
        }

        public List<string> tacktraces;
        public List<string> debugMsg;
        public List<string> debugTypes;
        public string temp;

        public void ReceiveDebugMsg(string condition, string stacktrace, LogType type)
        {
            debugMsg.Add(condition);
            tacktraces.Add(stacktrace);
            debugTypes.Add(type.ToString());
            temp = condition;
        }

        public void ClearConsole()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
            Type logEntries = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
            clearConsoleMethod.Invoke(new object(), null);
            temp = null;
            debugMsg.Clear();
        }
    }

    #endregion

    #region 消息通知类

    public class Notifier : ChangeNotifier
    {
        public void SendMessage()
        {
            this.notifyListeners();
        }
    }

    #endregion

    public class ConsoleWindow : StatefulWidget
    {
        public Notifier message;

        public ConsoleWindow(Notifier message)
        {
            this.message = message;
        }

        public override State createState()
        {
            return new ConsoleWindowState();
        }
    }

    public class ConsoleWindowState : State<ConsoleWindow>
    {
        public override void initState()
        {
            base.initState();

            widget.message.addListener(ChangeState);
            //debugMsg.Add(new Text("aaaa"));
        }

        public List<Widget> debugMsg = new List<Widget>();
        public string filterStr="";
        public void ChangeState()
        {
            lock (debugMsg)
            {
                if (DebugModel.Ins.temp.Contains(filterStr))
                {
                    debugMsg.Add(
                        new Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: new Widget[]
                            {
                                new Text(data: DebugModel.Ins.temp, maxLines: 2),
                                new Divider(color: Colors.blue),
                            }.ToList()
                        )
                    );
                }
            }
            setState();
        }

        private void StringFilter(string keyWords)
        {
            filterStr = keyWords;
            if (!string.IsNullOrEmpty(keyWords))
            {
                lock (debugMsg)
                {
                    debugMsg.Clear();

                    DebugModel.Ins.debugMsg.ForEach(x =>
                    {
                        if (x.Contains(keyWords))
                        {
                            debugMsg.Add(
                                new Column(
                                    crossAxisAlignment: CrossAxisAlignment.start,
                                    children: new Widget[]
                                    {
                                        new Text(data: x, maxLines: 2),
                                        new Divider(color: Colors.blue),
                                    }.ToList()
                                )
                            );
                        }
                    });
                }
               
                setState();
            }
        }

        public override Widget build(BuildContext context)
        {
            return new MaterialApp(
                theme: new ThemeData(
                    appBarTheme: new AppBarTheme(color: Colors.cyan),
                    cardTheme: new CardTheme(color: Colors.white, elevation: 2.0f)
                ),
                home: new Scaffold(
                    appBar: new AppBar(
                        title: new Text("CustomConsole")
                    ),
                    drawer: null,
                    body: new Container(
                        child:new Column (
                            
                            children: new Widget[]
                            {
                                new TextField(onChanged:StringFilter), 
                                new Expanded(child:new SingleChildScrollView(
                                    padding: EdgeInsets.all(16.0f),
                                    child: new Column(children: debugMsg),
                                    reverse: false
                                )
                                )
                                
                            }.ToList()
                        )),
                    floatingActionButton: new FloatingActionButton(
                        child: new Icon(Icons.clear),
                        onPressed: () =>
                        {
                            this.setState(() =>
                            {
                                DebugModel.Ins.ClearConsole();
                                debugMsg.Clear();
                            });
                        }
                    )
                )
            );
        }
    }
}