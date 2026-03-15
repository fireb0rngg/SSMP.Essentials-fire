using HarmonyLib;
using SSMP;
using SSMP.Api.Command;
using SSMP.Api.Command.Client;
using SSMP.Api.Command.Server;
using SSMP_Utils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SSMP_Utils.Client.Commands
{
    internal class HelpCommand : IClientCommand
    {
        public bool AuthorizedOnly => false;
        public string Trigger => "/help";
        public string[] Aliases => [];
        public void Execute(string[] arguments)
        {
            //var f = AccessTools.GetFieldNames(typeof(SSMPPlugin));
            //Log.LogInfo(f.Count);
            //var asembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "SSMP");
            //var pluginType = asembly.GetType("SSMP.SSMPPlugin");
            SSMPPlugin ssmpPlugin = BepInEx.Bootstrap.Chainloader.ManagerObject.GetComponent<SSMPPlugin>();

            var f = ssmpPlugin.GetType().GetField(
    "_gameManager",
    BindingFlags.Instance | BindingFlags.NonPublic);

            if (f== null)
            {
                Log.LogError("FIELD NOT FOUND");
            }

            //var ssmpPlugin = BepInEx.Bootstrap.Chainloader.PluginInfos["ssmp"].Instance;
            if (ssmpPlugin == null) Log.LogError("no plugin");

            var gManager = Traverse.Create(ssmpPlugin);
            Log.LogInfo(gManager.GetValue());
            if (gManager.GetValue() == null) Log.LogError("no plugin???");

            foreach (var field in gManager.Properties())
            {
                Log.LogInfo(field);
            }
            foreach (var field in gManager.Fields())
            {
                Log.LogInfo(field);
            }

            gManager = gManager.Field("_gameManager");
            Log.LogInfo(gManager.GetValue());
            if (gManager.GetValue() == null) Log.LogError("no g manager");

            var cliManager = gManager.Property("_clientManager");
            if (cliManager.GetValue() == null) Log.LogError("no cli manager");

            var cmdManager = cliManager.Property("_commandManager");
            if (cmdManager.GetValue() == null) Log.LogError("no cmd manager");

            var cliCommands = cmdManager.Property("Commands").GetValue<Dictionary<string, ICommand>>();
            if (cliCommands == null) Log.LogError("no commands");

            gManager = Traverse.Create(ssmpPlugin).Field("_gameManager");
            var serverManager = gManager.Field("_serverManager");
            var srvCmdManager = gManager.Field("CommandManager");
            var serverCommands = srvCmdManager.Field("Commands").GetValue<Dictionary<string, IServerCommand>>();

            foreach (var command in cliCommands)
            {
                Client.LocalChat($"{command.Value.Trigger}");


            }
            foreach (var command in serverCommands)
            {
                Client.LocalChat($"{command.Value.Trigger}");
            }
        }
    }
}
