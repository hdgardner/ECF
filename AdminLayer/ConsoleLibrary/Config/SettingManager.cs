using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Mediachase.Web.Console.Interfaces;

namespace Mediachase.Web.Console.Config
{
    public class SettingManager
    {
        /// <summary>
        /// Creates the setting collection.
        /// </summary>
        /// <param name="settingsNode">The settings node.</param>
        /// <returns></returns>
        public static ModuleSettingCollection CreateSettingsCollection(XmlNode settingsNode)
        {
            if (settingsNode == null)
                return null;

            // Create list
            ModuleSettingCollection settingCol = new ModuleSettingCollection();
            XmlNodeList settings = settingsNode.SelectNodes("Setting");

            foreach (XmlNode setting in settings)
            {
                settingCol.Add(PopulateSettingRecursive(setting));
            }

            return settingCol;
        }

        /// <summary>
        /// Populates the setting recursive.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private static ModuleSetting PopulateSettingRecursive(XmlNode node)
        {
            // Create root action
            ModuleSetting setting = new ModuleSetting(node.Attributes["name"].Value, node.Attributes["value"].Value);
            BindSettingProperties(setting, node);

            // Populate children
            XmlNodeList settings = node.SelectNodes("Setting");
            foreach (XmlNode nodeChild in settings)
            {
                setting.Children.Add(PopulateSettingRecursive(nodeChild));
            }

            return setting;
        }

        /// <summary>
        /// Binds the setting properties.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <param name="node">The node.</param>
        private static void BindSettingProperties(ModuleSetting setting, XmlNode node)
        {
            // Bind all attributes
            foreach (XmlAttribute attr in node.Attributes)
            {
                setting.Attributes[attr.Name] = attr.Value;
            }
        }
    }
}
