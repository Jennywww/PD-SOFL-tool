using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using XMLFormsApp.Model;

namespace XMLFormsApp.Common
{
    public enum Model 
    {
        OLD,
        NEW,
    }

    public class ProcessNodeHelper
    {
        public static Model model = Model.OLD;
        private const string CircleErrNodeFeatureName = "error-circle-feature";
        private const string CircleErrNodeProcessNamePrefix = "error-circle-node-";
        /**
//         * 获取输入或输出
//         *
//         * @param feature
//         * @param startFlag
//         * @param endFlag
//         * @param isInput 是否是输入 如果是输入，就应该取process中的第一个输入，如果是输出就应该取process中的最后一个输出
//         * @return
//         */
        public static List<List<string>> getinputsoroutputs(string process)
        {
            List<List<string>> result = new List<List<string>>();

            string sstr = process;
            if (string.IsNullOrEmpty(sstr))
            {
                return result;
            }
            string[] sarray = sstr.Split('|');
            foreach (var item in sarray)
            {
                List<string> varnames = new List<string>();
                string[] vararrays = item.Split(',');
                foreach (var varname in vararrays)
                {
                    string name = varname.Split(':')[0];
                    varnames.Add(name.Trim());
                }
                result.Add(varnames);
            }
            return result;
        }

        public static List<ProcessNode> CreateProcessNode(Process process)
        {
            List<ProcessNode> nodes = new List<ProcessNode>();
            if (process == null)
            {
                return nodes;
            }
            List<List<String>> inputs = getinputsoroutputs(process.Input);
            List<List<String>> outputs = getinputsoroutputs(process.Output);
            string processPost = process.post;
            int inputIndex = 0;
            int outputIndex = 0;
            foreach (var input in inputs)
            {
                outputIndex = 0;
                inputIndex++;
                foreach (var output in outputs)
                {
                    outputIndex++;
                    bool isMatch = false;
                    // 判断输入输出是否匹配，i和o只要存在一个匹配，那么input和output就是匹配的，因为input/output的值是逗号分隔的值
                    foreach (var i in input)
                    {

                        if (isMatch)
                        {
                            break;// 此输入输出已经匹配上了，终止此轮循环
                        }
                        foreach (var o in output)
                        {
                            if (model == Model.OLD)
                            {
                                string pattern1 = "[\\s|\\S]*.*" + i + ".*=.*" + o + ".*[\\s|\\S]*";
                                string pattern2 = "[\\s|\\S]*.*" + o + ".*=.*" + i + ".*[\\s|\\S]*";
                                string pattern3 = "[\\s|\\S]*.*" + i + ".*->.*" + o + ".*[\\s|\\S]*";
                                if (Regex.IsMatch(processPost, pattern1) || Regex.IsMatch(processPost, pattern2) || Regex.IsMatch(processPost, pattern3))
                                {
                                    ProcessNode node = new ProcessNode();
                                    node.FeatureName = process.FeaturnName;
                                    node.ProcessName = process.ProcessName;
                                    node.XmlNode = process.XmlNode;
                                    node.Input = input;
                                    node.Output = output;
                                    node.InputIndex = inputIndex;
                                    node.OutputIndex = outputIndex;
                                    node.Process = process;
                                    process.ProcessNodes.Add(node);
                                    nodes.Add(node);
                                    // 存在已能够匹配上
                                    isMatch = true;
                                    break;// 循环
                                }
                            }
                            else if (model == Model.NEW)
                            {
                                Scenarios scenarios = process.Scenarioses.Find(a => (a.G.Contains(i) && a.D.Contains(o)));
                                if (scenarios != null)
                                {
                                    ProcessNode node = new ProcessNode();
                                    node.FeatureName = process.FeaturnName;
                                    node.ProcessName = process.ProcessName;
                                    node.XmlNode = process.XmlNode;
                                    node.Input = input;
                                    node.Output = output;
                                    node.InputIndex = inputIndex;
                                    node.OutputIndex = outputIndex;
                                    node.Process = process;
                                    process.ProcessNodes.Add(node);
                                    nodes.Add(node);
                                    // 存在已能够匹配上
                                    node.Scenarios = scenarios;
                                    isMatch = true;
                                    break;// 循环
                                }
                            }
                            
                        }
                    }
                }
            }
            return nodes;
        }
        public static List<Dictionary<string, ProcessNode>> FeatureCombine(List<Dictionary<string, ProcessNode>> feature1, List<Dictionary<string, ProcessNode>> feature2,
                bool isFirstCheck)
        {
            Dictionary<int, List<string>> outputVarMap = GetSingleFeatureInputOrOuputVars(feature1, false);
            Dictionary<int, List<string>> inputVarMap = GetSingleFeatureInputOrOuputVars(feature2, true);

            List<Dictionary<string, ProcessNode>> retLinkMapList = new List<Dictionary<string, ProcessNode>>();
            List<Dictionary<string, ProcessNode>> mergedLinkMapList = new List<Dictionary<string, ProcessNode>>();// 已经合并过的链路
            if ((feature1 == null) || (feature1.Count < 0))
            {
                retLinkMapList.AddRange(feature2);
                return retLinkMapList;
            }
            if ((feature2 == null) || (feature2.Count <= 0))
            {
                retLinkMapList.AddRange(feature1);
                return retLinkMapList;
            }

            // 检查输出或者输入是否有关系
            bool hasRela = false;
            foreach (var outputLinkIndex in outputVarMap.Keys)
            {
                foreach (var inputLinkIndex in inputVarMap.Keys)
                {
                    // 之前没有往结果集中添加过 并且没有合并过
                    if (!retLinkMapList.Contains(feature1[outputLinkIndex]) && !mergedLinkMapList.Contains(feature1[outputLinkIndex]))
                    {
                        retLinkMapList.Add(feature1[outputLinkIndex]);
                    }
                    if (!retLinkMapList.Contains(feature2[inputLinkIndex]) && !mergedLinkMapList.Contains(feature2[inputLinkIndex]))
                    {
                        retLinkMapList.Add(feature2[inputLinkIndex]);
                    }

                    // 判断链路的输出和输入是否有关系
                    if (CheckVarsHasRela(outputVarMap[outputLinkIndex], inputVarMap[inputLinkIndex]))
                    {
                        hasRela = true;// 先后顺序确定，这两个feature的合并就不在考虑另外一种顺序了
                        Dictionary<String, ProcessNode> tempLinkMap = CopyNodeLinkMap(feature1[outputLinkIndex]);
                        foreach (var item in feature2[inputLinkIndex])
                        {
                            tempLinkMap.Add(item.Key, item.Value);      // 将feature2的第inputLinkIndex链路添加到feature1的第outputLinkIndex链路后面

                        }
                        retLinkMapList.Add(tempLinkMap);
                        retLinkMapList.Remove(feature1[outputLinkIndex]);// 把将要返回的结果集录 删除此条已合并的链路
                        retLinkMapList.Remove(feature2[inputLinkIndex]);// 把将要返回的结果集录 删除此条已合并的链路
                        mergedLinkMapList.Add(feature1[outputLinkIndex]);// 把此链路添加到已合并的记录中
                        mergedLinkMapList.Add(feature2[inputLinkIndex]);// 把此链路添加到已合并的记录中
                    }
                }
            }

            // hasRela为true，说明有关系，说明当前方向存在输出输入关系；或者没有关系，但是这已经是第二查找，所以也就直接返回了
            if (hasRela || !isFirstCheck)
            {
                return retLinkMapList;// 关系
            }
            // 第二次检查时，feature2与feature1的参数位置调换一下，就相当于换一种方向检查了
            return FeatureCombine(feature2, feature1, false);
        }
        /**
         * 检查两个变量集是否存在关系
         *
         * @param var1
         * @param var2
         * @return 是否有关系
         */
        public static bool CheckVarsHasRela(List<string> var1, List<string> var2)
        {
            foreach (var v1 in var1)
            {
                foreach (var v2 in var2)
                {
                    if (v1.Equals(v2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static Dictionary<int, List<String>> GetSingleFeatureInputOrOuputVars(List<Dictionary<string, ProcessNode>> feature, bool isGetInput)
        {
            Dictionary<int, List<String>> ret = new Dictionary<int, List<String>>();
            List<Dictionary<string, ProcessNode>> processNodes = feature;
            if ((feature == null) || (processNodes.Count <= 0))
            {
                return ret;
            }
            int index = 0;
            foreach (var linkMap in processNodes)
            {
                List<string> keyArrays = new List<string>();

                keyArrays.AddRange(linkMap.Keys);
                //取出模块中第一个和最后一个
                string firstKey = keyArrays[0];
                string lastKey = keyArrays[keyArrays.Count - 1];
                if (isGetInput)
                {
                    List<string> inputVars = linkMap[firstKey].Input;
                    if (inputVars.Count > 0)
                    {
                        ret.Add(index++, inputVars);
                    }
                }
                else
                {
                    List<string> outputVars = linkMap[lastKey].Output;
                    if (outputVars.Count > 0)
                    {
                        ret.Add(index++, outputVars);
                    }
                }
            }
            return ret;
        }
        public static Dictionary<string, ProcessNode> CopyNodeLinkMap(Dictionary<string, ProcessNode> nodeLinkMap)
        {
            Dictionary<string, ProcessNode> newNodeLinkMap = new Dictionary<string, ProcessNode>();

            foreach (var key in nodeLinkMap.Keys)
            {
                newNodeLinkMap.Add(key, nodeLinkMap[key]);
            }
            return newNodeLinkMap;
        }
        public static List<Dictionary<string, ProcessNode>> GetSingleFeatureAllLinkList(Dictionary<string, List<ProcessNode>> processMap, bool isPrint)
        {
            List<ProcessNode> firstProcessNode = GetFirstProcessNode(processMap);
            List<Dictionary<string, ProcessNode>> tempRetList = new List<Dictionary<string, ProcessNode>>();
            foreach (var node in firstProcessNode)
            {
                Dictionary<string, ProcessNode> nodeLinkMap = new Dictionary<string, ProcessNode>();
                List<Dictionary<string, ProcessNode>> linkMapList = GetSingleProcessNodeLinkMapList(node, nodeLinkMap);
                tempRetList.AddRange(linkMapList);
            }

            // 移除环状链路
            List<Dictionary<string, ProcessNode>> retList = new List<Dictionary<string, ProcessNode>>();
            foreach (var linkMap in tempRetList)
            {
                foreach (var item in linkMap.Keys)
                {
                    bool f = true;
                    if (item.StartsWith(CircleErrNodeProcessNamePrefix))
                    {
                        f = false;
                    }
                    if (f)
                    {
                        retList.Add(linkMap);
                    }
                }
            }

            return retList;
        }
        public static List<ProcessNode> GetFirstProcessNode(Dictionary<string, List<ProcessNode>> processMap)
        {
            List<ProcessNode> firstProcessNode = new List<ProcessNode>();
            foreach (var item in processMap)
            {
                foreach (var node in item.Value)
                {
                    if (node.PreNode.Count <= 0)
                    {
                        firstProcessNode.Add(node);
                    }
                }
            }
            return firstProcessNode;
        }
        public static List<Dictionary<string, ProcessNode>> GetSingleProcessNodeLinkMapList(ProcessNode node, Dictionary<string, ProcessNode> nodeLinkMap)
        {
            List<Dictionary<string, ProcessNode>> ret = new List<Dictionary<string, ProcessNode>>();
            ret.Add(nodeLinkMap);
            if (nodeLinkMap.ContainsKey(node.ProcessName))
            {
                // 出现循环节点，
                ProcessNode circleErrNode = new ProcessNode();
                circleErrNode.FeatureName = CircleErrNodeFeatureName;
                circleErrNode.ProcessName = CircleErrNodeProcessNamePrefix + node.ProcessName;
                nodeLinkMap.Add(CircleErrNodeProcessNamePrefix + node.ProcessName, circleErrNode);
                return ret;
            }

            nodeLinkMap.Add(node.ProcessName, node);
            if (node.NextNodes.Count <= 0)
            {
                return ret;// 没有后继节点了
            }
            foreach (var item in node.NextNodes)
            {
                Dictionary<string, ProcessNode> nextNodeLinkMap = CopyNodeLinkMap(nodeLinkMap);
                List<Dictionary<string, ProcessNode>> nextNodeRet = GetSingleProcessNodeLinkMapList(item, nextNodeLinkMap);
                ret.AddRange(nextNodeRet);
            }

            ret.Remove(nodeLinkMap);// 移除掉重复的子链路
            return ret;
        }
        public static List<ProcessRow> PrintSingleFeatureLinkMapList(List<Dictionary<string, ProcessNode>> linkMapList)
        {
            List<ProcessRow> result = new List<ProcessRow>();
            foreach (var linkMap in linkMapList)
            {
                List<string> str = new List<string>();
                ProcessRow processRow = new ProcessRow();
                List<string> path = new List<string>();
                foreach (var item in linkMap.Values)
                {
                    path.Add(item.FeatureName);
                    processRow.ProcessNodes.Add(item);
                    string inputStr = "[" + string.Join(",", item.Input) + "]";
                    string outputStr = "[" + string.Join(",", item.Output) + "]";
                    //string tempStr = inputStr + item.FeatureName+ ":"+item.ProcessName + item.InputIndex + item.OutputIndex + outputStr;
                    string tempStr = inputStr + item.ProcessName + item.InputIndex + item.OutputIndex + outputStr;
                    item.str = tempStr;
                    str.Add(tempStr);
                }
                string linkStr = string.Join("->", str);
                Console.WriteLine(linkStr);
                processRow.Post = linkStr;
                if (result.Find(a => (linkStr.Equals(a.Post))) == null)
                {
                    result.Add(processRow);
                }
            }
            // 打印链路
            return result;
        }

        public static void printToConsole(string message, bool activate)
        {
            //final MessageConsoleStream printer = ConsoleFactory.getConsole().newMessageStream();
            //printer.setActivateOnWrite(activate);
            //printer.println(message);
            Console.WriteLine(message);
        }
        //输出一个feature 里的process关系
        public static void PrintSingleProcessInputOutputRela(Dictionary<string, List<ProcessNode>> processMap)
        {
            foreach (var processName in processMap.Keys)
            {
                foreach (var node in processMap[processName])
                {
                    string nodeStr = "[" + string.Join(",", node.Input) + "]" + node.ProcessName + "|" + node.InputIndex + "|" + node.OutputIndex + "["
                     + string.Join(",", node.Output) + "]";
                    printToConsole(nodeStr, true);
                }
            }
        }
        public static void LeafCombine(List<string> featureNames, string retFeatureName, Dictionary<string, List<Dictionary<string, ProcessNode>>> featureLinkMap)
        {
            List<Dictionary<string, ProcessNode>> ret = new List<Dictionary<string, ProcessNode>>();
            if ((featureNames == null) || (featureNames.Count <= 0))
            {
                return;
            }

            if (featureNames.Count == 1)
            {
                return;
            }
            List<Dictionary<string, ProcessNode>> value = null;
            if (featureLinkMap.TryGetValue(featureNames[0],out value)) {
                ret.AddRange(value);
            }
            for (int i = 1; i < featureNames.Count(); i++)
            {
                List<Dictionary<string, ProcessNode>> v = null;
                featureLinkMap.TryGetValue(featureNames[i],out v);
                List<Dictionary<string, ProcessNode>> tempRet = FeatureCombine(ret, v,true);
                ret.Clear();
                ret.AddRange(tempRet);
            }
            if (!featureLinkMap.ContainsKey(retFeatureName))
            {
                featureLinkMap.Add(retFeatureName, new List<Dictionary<string, ProcessNode>>());
            }
            featureLinkMap[retFeatureName].Clear();
            featureLinkMap[retFeatureName].AddRange(ret);// 重新设置当前feature的链路
        }
        /**
         * 获取一个feature中所有process和其对应的输入输出关系
         *
         * @param featureName
         * @param processList
         * @return
         */
        public static Dictionary<string, List<ProcessNode>> ProcessCombineNodeLink(string featureName, Dictionary<string, Process> processList)
        {
            Dictionary<string, List<ProcessNode>> processMap = new Dictionary<string, List<ProcessNode>>();
            if ((processList == null) || (processList.Count <= 0))
            {
                return processMap;
            }
            foreach (var process in processList.Values)
            {
                List<ProcessNode> nodes = CreateProcessNode(process);
                if (nodes.Count <= 0)
                {
                    continue;
                }
                processMap.Add(process.ProcessName, nodes);
            }
            if (processMap.Count <= 0)
            {
                return processMap;
            }

            // process的两两组合全部都要判断一遍，最后再移除环状链路
            List<string> keys = processMap.Keys.ToList();
            int jizhunIndex = 0;
            int i = 1;
            for (i = jizhunIndex + 1; i < keys.Count; i++)
            {
                string jizhunProcessName = keys[jizhunIndex];
                string processName = keys[i];
                List<ProcessNode> combineRet = new List<ProcessNode>();
                SetUpFeatureProcessRela(processMap[jizhunProcessName], processMap[processName]);
                if (i == (keys.Count - 1))
                {
                    jizhunIndex += 1;
                    i = jizhunIndex;
                }
            }

            printToConsole(featureName, true);
            // 打印单个proces的输入输出关系
            PrintSingleProcessInputOutputRela(processMap);
            printToConsole("", true); //
                                      // 获取单个feature的有效链路集合
            GetSingleFeatureAllLinkList(processMap, true);

            return processMap;
        }
        /**
         * 获取feature的输入输出关系链路集合
         *
         * @param projectBaseDir
         * @param projectName
         * @param selectedFeatureMap
         * @return
         */
        public static Dictionary<string, List<Dictionary<string, ProcessNode>>> GetFeatureLinkMapList(List<Feature> features)
        {
            // 获取单个feature内部所有process的节点输入输出关系
            Dictionary<String, Dictionary<string, List<ProcessNode>>> featureProcessNodeMap = new Dictionary<String, Dictionary<string, List<ProcessNode>>>();
            foreach (var feature in features)
            {
                featureProcessNodeMap.Add(feature.Name, ProcessCombineNodeLink(feature.Name, feature.Processes));
            }
            Dictionary<string, List<Dictionary<string, ProcessNode>>> featureLinkMap = new Dictionary<string, List<Dictionary<string, ProcessNode>>>();
            // 获取单个feature的链路关系
            foreach (var featureName in featureProcessNodeMap.Keys)
            {
                featureLinkMap.Add(featureName, GetSingleFeatureAllLinkList(featureProcessNodeMap[featureName], false));
            }
            return featureLinkMap;
        }
        /**
         * 给两个process之间设置关系
         *
         * @param process1
         * @param process2
         */
        public static void SetUpFeatureProcessRela(List<ProcessNode> process1, List<ProcessNode> process2)
        {
            bool hasCombined = false;
            if ((process1.Count <= 0) || (process2.Count <= 0))
            {
                return;
            }

            int preOrNext = 0;// process1是前驱节点还是后继节点，0是初始值，表示还未确定，1：前驱，2：后继。因为一旦顺序确定之后，后面存在相反的输入输出关系也忽略掉，避免引起环
            foreach (var node1 in process1)
            {
                foreach (var node2 in process2)
                {
                    // 检查两个node是否存在输入输出关系
                    // 检查node1的输出与node2的输入是否有关系
                    if (CheckVarsHasRela(node1.Output, node2.Input) && ((preOrNext == 0) || (preOrNext == 1)))
                    {
                        // 节点合并，node1是node2的前驱节点
                        preOrNext = 1;
                        node1.NextNodes.Add(node2);
                        node2.PreNode.Add(node1);
                        continue;

                    }
                    else if (CheckVarsHasRela(node2.Output, node1.Input) && ((preOrNext == 0) || (preOrNext == 2)))
                    {
                        preOrNext = 2;
                        node2.NextNodes.Add(node1);
                        node1.PreNode.Add(node2);
                        continue;
                    }
                }
            }
        }
        // 叶子节点组合
        public static string LeafNodeCombine(List<Feature> 
            features,Feature rootFeature, ref Dictionary<string, List<Dictionary<string, ProcessNode>>> featureLinkMap)
        {
            featureLinkMap = ProcessNodeHelper.GetFeatureLinkMapList(features);
            List<Dictionary<string, ProcessNode>> linkMapList = new List<Dictionary<string, ProcessNode>>();

            // 获取所有的叶子节点
            Dictionary<Feature, int> allLeafMap = GetAllLeafNode(rootFeature);
            Dictionary<Feature, int> backAllLeafMap = new Dictionary<Feature, int>();
            // 获取树的最大深度
            int maxLevel = 0;
            foreach (var item in features)
            {
                if (item.Level > maxLevel)
                    maxLevel = item.Level;
            }
            for (int i = 1; i <= maxLevel; i++)
            {
                for (int j = maxLevel; j >= 0; j--)
                {
                    while (true)
                    {
                        // 获取一个最深的叶子节点
                        Feature maxLevelLeaf = GetMaxLevelLeaf(allLeafMap, j);
                        if (maxLevelLeaf == null)
                        {
                            break;// 当前深度已经没有需要处理的节点了
                        }
                        // 获取最深叶子节点 到指定层级的父节点
                        Feature parent = GetSpecialLevelCountParentFeature(maxLevelLeaf, i);
                        // 获取此父节点下的所有叶子节点
                        Dictionary<Feature,int> leafMap = GetAllLeafNode(parent);
                        // 求交集
                        var leafList = leafMap.Keys.ToHashSet();
                        var allLeafList = allLeafMap.Keys.ToHashSet();

                        var intersectList = leafList.Intersect(allLeafList);
                        // 处理输入输出关系的合并
                        List<string> featureNameList = new List<string>();
                        foreach (var item in intersectList)
                        {
                            featureNameList.Add(item.Name);
                        }
                        string combineFeatureName = GetCombineFeatureName(intersectList.ToList());
                        ProcessNodeHelper.LeafCombine(featureNameList, combineFeatureName, featureLinkMap);
                        // allLeafMap删除掉已经合并的叶子节点(即交集结果)，防止下次循环在去到此结果
                        foreach (var item in intersectList)
                        {
                            allLeafMap.Remove(item);
                        }
                        // 将合并结果添加到backAllLeafMap，用于下一此深度处理
                        maxLevelLeaf.Name=combineFeatureName;
                        if (!backAllLeafMap.ContainsKey(maxLevelLeaf))
                        {
                            backAllLeafMap.Add(maxLevelLeaf, j);
                        }
                        else
                        {
                            backAllLeafMap[maxLevelLeaf] = j;
                        }
                    }
                }
                if (allLeafMap.Count != 0)
                {
                    Console.WriteLine("处理深度层次i=" + i + "时，allLeafMap结果不为空");// 打印合并结果
                }
                allLeafMap.Clear();
                foreach (var item in backAllLeafMap)
                {
                    allLeafMap.Add(item.Key,item.Value);
                }
                backAllLeafMap.Clear();// 清空
            }
            string retFeatureName = "";

            foreach (var item in allLeafMap)
            {
                retFeatureName = item.Key.Name;
                break;
            }
            if (featureLinkMap.Count != 0)
            {
                ProcessNodeHelper.PrintSingleFeatureLinkMapList(featureLinkMap[retFeatureName]);
            }
            return retFeatureName;// 打印合并结果
        }

        // 深度优先 获取一个节点
        public static Feature GetMaxLevelLeaf(Dictionary<Feature,int> leafMap, int maxLevel)
        {
            if ((leafMap == null) || (leafMap.Count <= 0))
            {
                return null;
            }
            // 获取最大深度的所有叶子节点
            foreach (var item in leafMap)
            {
                if (item.Value == maxLevel)
                {
                    return item.Key;
                }
            }
            return null;
        }



        // 获取leaf节点合并的名称
        public static string GetCombineFeatureName(List<Feature> leafArray)
        {
            if ((leafArray == null) || (leafArray.Count <= 0))
            {
                return "";
            }
            List<string> featureNameList = new List<string>();
            foreach (var item in leafArray)
            {
                featureNameList.Add(item.Name);
            }
            if (featureNameList.Count == 1)
            {
                return featureNameList[0];
            }
            string ret = string.Join(",", featureNameList);
            if (string.IsNullOrEmpty(ret))
            {
                return "";
            }
            return "(" + ret + ")";
        }

        //// 获取指定节点的指定层次数的父节点
        public static Feature GetSpecialLevelCountParentFeature(Feature feature, int levelCount)
        {
            if (levelCount <= 0)
            {
                return feature;
            }
            Feature parent = feature.ParentFeature;
            if (parent == null)
            {
                return feature;// 当前节点是根节点，不存在父节点，所以就直接返回
            }
            levelCount--;
            return GetSpecialLevelCountParentFeature(parent, levelCount);
        }

        // 获取指定节点下的所有叶子节点

        public static Dictionary<Feature, int> GetAllLeafNode(Feature feature)
        {
            Dictionary<Feature, int> leefNode = new Dictionary<Feature, int>();
            if (feature == null)
            {
                return leefNode;
            }
            if (!feature.haveChild())
            {
                leefNode.Add(feature, feature.Level);
                return leefNode;
            }
            List<Feature> childs = feature.ChildFeatures;
            foreach (var item in childs)
            {
                Dictionary<Feature, int> childLeef = GetAllLeafNode(item);
                foreach (var c in childLeef)
                {
                    leefNode.Add(c.Key,c.Value);
                }
            }
            return leefNode;
        }


        public static string FilterXML(string xml,string valueName)
        {
            string start ="<"+ valueName + ">";
            string end = "</" + valueName + ">";
            xml = xml.Replace(start, start + "<![CDATA[");
            xml = xml.Replace(end, "]]>"+end);
            return xml;
        }

        public static string Standardization(XmlDocument xml)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlNodeList featureXmlNode = xml.DocumentElement.SelectNodes("feature");
            foreach (XmlNode item in featureXmlNode)
            {
                stringBuilder.Append(string.Format("feature {0};\r\n", item.Attributes["name"].Value));
                XmlNode module = item.SelectSingleNode("module");
                stringBuilder.Append(string.Format("module {0};\r\n", module.Attributes["name"].Value));

                XmlNode consts = module.SelectSingleNode("consts");
                stringBuilder.Append("\tconsts\n\t\t");
                if (consts!=null)
                    stringBuilder.Append(string.Format("{0}\r\n", consts.InnerText));

                XmlNode types = module.SelectSingleNode("types");
                stringBuilder.Append("\ttypes\n\t\t");
                if(types!=null)
                    stringBuilder.Append(string.Format("{0}\r\n", types.InnerText.Replace("\n","\r\n").Trim()));

                XmlNode var = module.SelectSingleNode("var");
                stringBuilder.Append("\tvar\n\t\t");
                if (var != null)
                    stringBuilder.Append(string.Format("{0}\r\n", var.InnerText.Replace("\n", "\r\n").Trim()));

                XmlNode inv = module.SelectSingleNode("inv");
                stringBuilder.Append("\tinv\n\t\t");
                if (inv != null) {
                    stringBuilder.Append(string.Format("{0}\r\n", inv.InnerText.Replace("\n", "\r\n").Trim()));
                }

                XmlNodeList processes = module.SelectNodes("process");
                if (processes != null)
                {
                    foreach (XmlNode process in processes)
                    {
                        XmlNode inputs = process.SelectSingleNode("inputs");
                        XmlNode outputs = process.SelectSingleNode("outputs");
                        XmlNode ext = process.SelectSingleNode("ext");
                        XmlNode pre = process.SelectSingleNode("pre");
                        stringBuilder.Append(string.Format("\tprocess {0}({1}){2}\r\n", process.Attributes["name"].Value, inputs.InnerText, outputs.InnerText));
                        if (ext != null)
                        {
                            stringBuilder.Append(string.Format("\text wr {0}\r\n", ext.InnerText));
                        }
                        stringBuilder.Append(string.Format("\tpre {0}\r\n", pre.InnerText));
                        XmlNodeList scenarioses = process.SelectNodes("post/scenarios");
                        stringBuilder.Append("\tpost\r\n");
                        foreach (XmlNode scenarios in scenarioses)
                        {
                            XmlNode C = scenarios.SelectSingleNode("G");
                            stringBuilder.Append(string.Format("\t\t{0}\r\n", C.InnerText));
                            XmlNode D = scenarios.SelectSingleNode("D");
                            stringBuilder.Append(string.Format("\t\t{0}\r\n", D.InnerText));
                        }
                    }
                    stringBuilder.Append(string.Format("\tend_process;\r\n"));
                    stringBuilder.Append(string.Format("end_module;\r\n"));
                }
            }
            return stringBuilder.ToString();
        }
        public static string Standardization(XmlDocument xml,string feature)
        {

            return MergeFeature(xml, feature, feature);
            //StringBuilder stringBuilder = new StringBuilder();
            //XmlNodeList featureXmlNode = xml.DocumentElement.SelectNodes("feature");
            //foreach (XmlNode item in featureXmlNode)
            //{
            //    if (feature.Equals(item.Attributes["name"].Value))
            //    {
            //        stringBuilder.Append(string.Format("feature {0};\r\n", item.Attributes["name"].Value));
            //        XmlNode module = item.SelectSingleNode("module");
            //        stringBuilder.Append(string.Format("module {0};\r\n", module.Attributes["name"].Value));

            //        XmlNode consts = module.SelectSingleNode("consts");
            //        stringBuilder.Append(string.Format("\tconsts\n\t\t{0}\r\n", consts.InnerText));
            //        XmlNode types = module.SelectSingleNode("types");
            //        stringBuilder.Append(string.Format("\ttypes\r\n\t\t{0}\r\n", types.InnerText.Replace(" ","").Replace("\n", "\r\n\t\t").Trim()));
            //        XmlNode var = module.SelectSingleNode("var");
            //        if (var != null)
            //            stringBuilder.Append(string.Format("\tvar\r\n\t\t{0}\r\n", var.InnerText.Replace("\n", "\r\n").Trim()));
            //        XmlNode inv = module.SelectSingleNode("inv");
            //        if (inv != null)
            //        {
            //            stringBuilder.Append(string.Format("\tinv\r\n\t\t{0}\r\n", inv.InnerText.Replace("\n        ", "\r\n\t\t").Trim()));
            //        }
            //        XmlNodeList processes = module.SelectNodes("process");
            //        if (processes != null)
            //        {
            //            foreach (XmlNode process in processes)
            //            {
            //                XmlNode inputs = process.SelectSingleNode("inputs");
            //                XmlNode outputs = process.SelectSingleNode("outputs");
            //                XmlNode ext = process.SelectSingleNode("ext");
            //                XmlNode pre = process.SelectSingleNode("pre");
            //                stringBuilder.Append(string.Format("\tprocess {0}({1}){2}\r\n", process.Attributes["name"].Value, inputs.InnerText, outputs.InnerText));
            //                if (ext != null)
            //                {
            //                    stringBuilder.Append(string.Format("\t\text wr {0}\r\n", ext.InnerText));
            //                }
            //                stringBuilder.Append(string.Format("\t\tpre {0}\r\n", pre.InnerText));
            //                XmlNodeList scenarioses = process.SelectNodes("post/scenarios");
            //                stringBuilder.Append("\t\tpost\r\n");
            //                foreach (XmlNode scenarios in scenarioses)
            //                {
            //                    XmlNode C = scenarios.SelectSingleNode("G");
            //                    stringBuilder.Append(string.Format("\t\t\t{0}\r\n", C.InnerText));
            //                    XmlNode D = scenarios.SelectSingleNode("D");
            //                    stringBuilder.Append(string.Format("\t\t\t{0}\r\n", D.InnerText));
            //                }
            //            }
            //            stringBuilder.Append(string.Format("\tend_process;\r\n"));
            //            stringBuilder.Append(string.Format("end_module;\r\n"));
            //        }
            //    }
            //}
            //return stringBuilder.ToString();
        }

        public static string FindFeatureAttribute(XmlDocument xml, string feature,string Attribute)
        {
            StringBuilder stringBuilder = new StringBuilder();
            XmlNodeList featureXmlNode = xml.DocumentElement.SelectNodes("feature");
            foreach (XmlNode item in featureXmlNode)
            {
                if (feature.Equals(item.Attributes["name"].Value))
                {
                    XmlNode types = item.SelectSingleNode(string.Format("module/{0}",Attribute));
                    if (types != null)
                    {
                        stringBuilder.Append(string.Format("\n\t\t{0}\r\n", types.InnerText.Replace("\n", "\r\n").Trim()));
                    }
                }
            }
            return stringBuilder.ToString();
        }
        public static string FindFeatureProcess(XmlDocument xml, string feature, char Separator='@')
        {
            //XmlNodeList featureXmlNode = xml.DocumentElement.SelectNodes("feature");
            ////Dictionary<string, string> result = new Dictionary<string, string>();
            //foreach (XmlNode item in featureXmlNode)
            //{
            //    if (feature.Equals(item.Attributes["name"].Value))
            //    {
            //        XmlNodeList processes = item.SelectNodes("module/process");
            //        if (processes != null)
            //        {
            //            foreach (XmlNode process in processes)
            //            {
            //                XmlNode inputs = process.SelectSingleNode("inputs");
            //                XmlNode outputs = process.SelectSingleNode("outputs");
            //                XmlNode ext = process.SelectSingleNode("ext");
            //                XmlNode pre = process.SelectSingleNode("pre");
            //                result.Add("inputs", inputs.InnerText);
            //                result.Add("outputs", outputs.InnerText);
            //                if (ext != null)
            //                {
            //                    result.Add("ext", ext.InnerText);
            //                }
            //                result.Add("pre", pre.InnerText);
            //                XmlNodeList scenarioses = process.SelectNodes("post/scenarios");
            //                StringBuilder stringBuilder = new StringBuilder();
            //                foreach (XmlNode scenarios in scenarioses)
            //                {
            //                    XmlNode C = scenarios.SelectSingleNode("C");
            //                    XmlNode D = scenarios.SelectSingleNode("D");
            //                    stringBuilder.Append(string.Format("{0}\r{1}{2}", C.InnerText.Trim(),D.InnerText.Trim(), Separator));
            //                }
            //                result.Add("post", stringBuilder.ToString());
            //            }
            //        }
            //    }
            //}
            //return result;
            StringBuilder stringBuilder = new StringBuilder();
            XmlNodeList featureXmlNode = xml.DocumentElement.SelectNodes("feature");
            foreach (XmlNode item in featureXmlNode)
            {
                if (feature.Equals(item.Attributes["name"].Value))
                {
                    XmlNodeList processes = item.SelectNodes("module/process");
                    if (processes != null)
                    {
                        foreach (XmlNode process in processes)
                        {
                            XmlNode inputs = process.SelectSingleNode("inputs");
                            XmlNode outputs = process.SelectSingleNode("outputs");
                            XmlNode ext = process.SelectSingleNode("ext");
                            XmlNode pre = process.SelectSingleNode("pre");
                            stringBuilder.Append(string.Format("\tprocess {0}({1}){2}\r\n", process.Attributes["name"].Value, inputs.InnerText, outputs.InnerText));
                            if (ext != null)
                            {
                                stringBuilder.Append(string.Format("\t\text wr {0}\r\n", ext.InnerText));
                            }
                            stringBuilder.Append(string.Format("\t\tpre {0}\r\n", pre.InnerText));
                            XmlNodeList scenarioses = process.SelectNodes("post/scenarios");
                            stringBuilder.Append("\t\tpost\r\n");
                            foreach (XmlNode scenarios in scenarioses)
                            {
                                XmlNode C = scenarios.SelectSingleNode("G");
                                stringBuilder.Append(string.Format("\t\t\t{0}\r\n", C.InnerText));
                                XmlNode D = scenarios.SelectSingleNode("D");
                                stringBuilder.Append(string.Format("\t\t\t{0}\r\n", D.InnerText));
                            }
                            stringBuilder.Append(string.Format("\tend_process;\r\n"));
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }
        public static string RemoveBracket(string xml,string valueName)
        {
            string start = "<" + valueName + ">";
            string end = "</" + valueName + ">";
            xml = xml.Replace(start, valueName);
            xml = xml.Replace(end, "end_"+valueName);
            return xml;
        }
        public static string MergeType(XmlDocument currentXml, string features)
        {
            char[] split = { ',', ')', '(', ' ', '\r', '\n', '\0' };
            char[] split2 = { '\r', '\n', '\t' };
            string[] featureList = features.Split(split, StringSplitOptions.RemoveEmptyEntries);
            string composed = "composed of";
            string flagComposed = "||Ca%^U&sd@1!$";

            StringBuilder stringBuilder = new StringBuilder();
            Dictionary<string, string> veriates = new Dictionary<string, string>();
            List<string> composes = new List<string>();
            foreach (var featureName in featureList)
            {
                string type = ProcessNodeHelper.FindFeatureAttribute(currentXml, featureName,"types");
                string[] stype = type.Split(split2, StringSplitOptions.RemoveEmptyEntries);
                bool isComposed = false;
                StringBuilder composedVeriate = new StringBuilder();
                string ComposedName = "";
                foreach (string item in stype)
                {
                    string[] veriate = item.Split(':', '=', '\r', '\n', '\t');
                    if (isComposed)
                    {
                        if (item.Contains("end;"))
                        {
                            isComposed = false;
                            if (!veriates.ContainsKey(ComposedName))
                            {
                                veriates.Add(ComposedName, composedVeriate.ToString());
                            }
                            veriates[ComposedName] += composedVeriate.ToString();
                            composedVeriate.Clear();
                        }
                        else
                        {
                            if (veriate.Length >= 2 && !veriate[1].Contains(composed))
                            {
                                string veriateName = veriate[0].Trim();
                                if (!veriates.ContainsKey(veriateName))
                                {
                                    composedVeriate.Append(item.Trim() + "\r");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (veriate.Length >= 2 && !veriate[1].Contains(composed))
                        {
                            string veriateName = veriate[0].Trim();
                            if (!veriates.ContainsKey(veriateName))
                            {
                                veriates.Add(veriateName, veriate[1].Trim());
                            }
                        }
                        else if (veriate.Length >= 2 && veriate[1].Contains(composed))
                        {
                            string veriateName = veriate[0].Trim();
                            ComposedName = veriateName + flagComposed;
                            isComposed = true;
                        }
                    }
                }
            }
            StringBuilder e = new StringBuilder();
            foreach (var item in veriates)
            {
                if (item.Key.Contains(flagComposed))
                {
                    string name = item.Key.Replace(flagComposed, "");
                    string[] str = item.Value.Split('\r');
                    HashSet<string> v = new HashSet<string>(str);
                    foreach (var s in v)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            e.Append("\t\t\t"+s + "\r\n");
                        }
                    }
                    stringBuilder.Append("\t\t"+name + " = composed of\r\n" + e.ToString() + "\t\tend;\r\n");
                }
                else
                {
                    stringBuilder.Append("\t\t"+item.Key + " = " + item.Value+ "\r\n");
                }
                e.Clear();
            }
            Console.WriteLine(stringBuilder.ToString());
            return stringBuilder.ToString();
        }
        public static string MergeVar(XmlDocument currentXml, string features)
        {
            char[] split = { ',', ')', '(', ' ', '\r', '\n', '\0' };

            string[] featureList = features.Split(split, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder stringBuilder = new StringBuilder();
            Dictionary<string, string> veriates = new Dictionary<string, string>();

            foreach (var featureName in featureList)
            {
                string Var = ProcessNodeHelper.FindFeatureAttribute(currentXml, featureName,"var");
                string[] vars = Var.Split('\r', '\n', '\t');

                foreach (var item in vars)
                {
                    string[] var = item.Split(':');
                    if (var.Length == 2)
                    {
                        if (!veriates.ContainsKey(var[0]))
                        {
                            veriates.Add(var[0], item);
                        }
                    }
                }
            }
            foreach (var item in veriates)
            {
                stringBuilder.Append("\t\t"+item.Value.Trim() + "\r\n");
            }
            return stringBuilder.ToString();
        }

        public static string MergeInv(XmlDocument currentXml, string features)
        {
            char[] split = { ',', ')', '(', ' ', '\r', '\n', '\0' };

            string[] featureList = features.Split(split, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder stringBuilder = new StringBuilder();
            Dictionary<string, string> veriates = new Dictionary<string, string>();

            foreach (var featureName in featureList)
            {
                string Inv = ProcessNodeHelper.FindFeatureAttribute(currentXml, featureName,"inv");
                string[] vars = Inv.Split('\r', '\n', '\t');

                foreach (var item in vars)
                {
                    if (!veriates.ContainsKey(item))
                    {
                        veriates.Add(item, item);
                    }
                }
            }
            foreach (var item in veriates)
            {
                stringBuilder.Append("\t\t"+item.Value.Trim() + "\r\n");
            }
            return stringBuilder.ToString();
        }

        public static string MergeProcess(XmlDocument currentXml, string features)
        {
            char[] split = { ',', ')', '(', ' ', '\r', '\n', '\0' };

            string[] featureList = features.Split(split, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder stringBuilder = new StringBuilder();
            //Dictionary<string, string> inputs = new Dictionary<string, string>();
            //Dictionary<string, string> outputs = new Dictionary<string, string>();
            //Dictionary<string, string> ext = new Dictionary<string, string>();
            //Dictionary<string, string> pre = new Dictionary<string, string>();
            //Dictionary<string, string> post = new Dictionary<string, string>();
            //List<string> post = new List<string>();


            foreach (var featureName in featureList)
            {
                string process = ProcessNodeHelper.FindFeatureProcess(currentXml, featureName);
                Console.WriteLine(process);
                stringBuilder.Append("\t"+process.Trim()+ "\r\n");
                //Dictionary<string,string> process = ProcessNodeHelper.FindFeatureProcess(currentXml, featureName);
                //string []inputsSTR = process["inputs"].Split('|');
                //foreach (var item in inputsSTR)
                //{
                //    string []v = item.Split(':');
                //    if (v.Length == 2)
                //    {
                //        if (!inputs.ContainsKey(v[0]))
                //        {
                //            inputs.Add(v[0], item);
                //        }
                //    }
                //}

                //string[] outputsSTR = process["outputs"].Split('|');
                //foreach (var item in outputsSTR)
                //{
                //    string[] v = item.Split(':');
                //    if (v.Length == 2)
                //    {
                //        if (!outputs.ContainsKey(v[0]))
                //        {
                //            outputs.Add(v[0], item);
                //        }
                //    }
                //}

                //if (!ext.ContainsKey(process["ext"]))
                //{
                //    ext.Add(process["ext"], process["ext"]);
                //}

                //if (!pre.ContainsKey(process["pre"]))
                //{
                //    pre.Add(process["pre"], process["pre"]);
                //}
                //string[] postSTR = process["post"].Split('@');
                //foreach (var item in postSTR)
                //{
                //    post.Add(item);
                //}
            }
            return stringBuilder.ToString();
        }

        public static string MergeFeature(XmlDocument currentXml, string features,string name)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Format("feature {0};\r\n", name));
            stringBuilder.Append(string.Format("module {0};\r\n", name));
            stringBuilder.Append(string.Format("\tconsts\r\n\t\t{0}\r\n", ""));
            stringBuilder.Append(string.Format("\ttypes\r\n\t\t{0}\r\n", MergeType(currentXml,features).Trim()));
            stringBuilder.Append(string.Format("\tvar\r\n\t\t{0}\r\n", MergeVar(currentXml, features).Trim()));
            stringBuilder.Append(string.Format("\tinv\r\n\t\t{0}\r\n", MergeInv(currentXml, features).Trim()));
            stringBuilder.Append(MergeProcess(currentXml, features));
            stringBuilder.Append(string.Format("end_module;\r\n"));
            return stringBuilder.ToString();
        }
        public static List<string> GetCalculate(string Order)
        {
            Stack<string> names = new Stack<string>();
            Stack<char> operation =new Stack<char>();
            List<string> result = new List<string>();
            string name = "";
            bool isName = false;
            for (int i = 0; i < Order.Length; i++)
            {
                if (Order[i] == ',' || Order[i] == '(')
                {
                    if (isName)
                    {
                        names.Push(name);
                        isName = false;
                        name = "";
                    }
                    operation.Push(Order[i]);
                }
                else if (Order[i] ==')')
                {
                    if (isName)
                    {
                        names.Push(name);
                        isName = false;
                        name = "";
                    }
                    while (operation.Peek() != '(')
                    {
                        char o = operation.Pop();
                        if (o == ',')
                        {
                            string a = names.Pop();
                            string b = names.Pop();
                            result.Add('(' + a + ',' + b + ')');
                            names.Push('(' + a + ',' + b + ')');
                        }
                    }
                    operation.Pop();
                }
                else
                {
                    isName = true;
                    name += Order[i];
                }
            }
            if (isName)
            {
                names.Push(name);
                isName = false;
                name = "";
            }
            while (operation.Count!=0)
            {
                char o = operation.Pop();
                if (o == ',')
                {
                    string a = names.Pop();
                    string b = names.Pop();
                    result.Add('(' + a + ',' + b + ')');
                    names.Push('(' + a + ',' + b + ')');
                }
            }
            return result;
        }
    }
}