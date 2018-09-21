using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Lemon;

namespace Lemon.Csv
{
    #region 类说明信息
    /// <summary>
    ///  <DL>
    ///  <DT><b>写CSV文件类,首先给CSV文件赋值,最后通过Save方法进行保存操作</b></DT>
    ///   <DD>
    ///    <UL> 
    ///    </UL>
    ///   </DD>
    ///  </DL>
    ///  <Author>yangzhihong</Author>    
    ///  <CreateDate>2006/01/16</CreateDate>
    ///  <Company></Company>
    ///  <Version>1.0</Version>
    /// </summary>
    #endregion
    public class CsvStreamWriter
    {
        private List<object> rowAL;        //行链表,CSV文件的每一行就是一个链
        private string fileName;        //文件名
        private Encoding encoding;      //编码

        public CsvStreamWriter()
        {
            this.rowAL = new List<object>();
            this.fileName = "";
            this.encoding = Encoding.UTF8;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">文件名,包括文件路径</param>
        public CsvStreamWriter(string fileName)
        {
            this.rowAL = new List<object>();
            this.fileName = fileName;
            this.encoding = Encoding.UTF8;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">文件名,包括文件路径</param>
        /// <param name="encoding">文件编码</param>
        public CsvStreamWriter(string fileName, Encoding encoding)
        {
            this.rowAL = new List<object>();
            this.fileName = fileName;
            this.encoding = encoding;
        }

        /// <summary>
        /// row:行,row = 1代表第一行
        /// col:列,col = 1代表第一列
        /// </summary>
        public string this[int row, int col]
        {
            set
            {
                //对行进行判断
                if (row <= 0)
                {
                    throw new Exception("行数不能小于0");
                }
                else if (row > this.rowAL.Count) //如果当前列链的行数不够，要补齐
                {
                    for (int i = this.rowAL.Count + 1; i <= row; i++)
                    {
                        this.rowAL.Add(new List<object>());
                    }
                }
                else
                {
                }
                //对列进行判断
                if (col <= 0)
                {
                    throw new Exception("列数不能小于0");
                }
                else
                {
                    List<object> colTempAL = (List<object>)this.rowAL[row - 1];

                    //扩大长度
                    if (col > colTempAL.Count)
                    {
                        for (int i = colTempAL.Count; i <= col; i++)
                        {
                            colTempAL.Add("");
                        }
                    }
                    this.rowAL[row - 1] = colTempAL;
                }
                //赋值
                List<object> colAL = (List<object>)this.rowAL[row - 1];

                colAL[col - 1] = value;
                this.rowAL[row - 1] = colAL;
            }
        }

        /// <summary>
        /// 文件名,包括文件路径
        /// </summary>
        public string FileName
        {
            set
            {
                this.fileName = value;
            }
        }

        /// <summary>
        /// 文件编码
        /// </summary>

        public Encoding FileEncoding
        {
            set
            {
                this.encoding = value;
            }
        }

        /// <summary>
        /// 获取当前最大行
        /// </summary>
        public int CurMaxRow
        {
            get
            {
                return this.rowAL.Count;
            }
        }

        /// <summary>
        /// 获取最大列
        /// </summary>
        public int CurMaxCol
        {
            get
            {
                int maxCol;

                maxCol = 0;
                for (int i = 0; i < this.rowAL.Count; i++)
                {
                    List<object> colAL = (List<object>)this.rowAL[i];

                    maxCol = (maxCol > colAL.Count) ? maxCol : colAL.Count;
                }

                return maxCol;
            }
        }

        public void SetData(List<object> dataDT, Type type)
        {
            if (dataDT == null)
            {
                throw new Exception("需要添加的表数据为空");
            }
            int curMaxRow;
            curMaxRow = this.rowAL.Count;
            IEnumerable<PropertyInfo> plist = type.GetRuntimeProperties();
            List<PropertyInfo> writeList = new List<PropertyInfo>();
            foreach (var p in plist)
            {
                if (type.GetTypeInfo().GetCustomAttribute<NotDataField>() != null)
                    continue;
                writeList.Add(p);
            }
            //添加标题
            for (int c = 1; c <= writeList.Count; c++)
            {
                this[1, c] = writeList[c - 1].Name;
            }
            //添加数据
            for (int i = 0; i < dataDT.Count; i++)
            {
                for (int j = 0; j < writeList.Count; j++)
                {
                    object pvOnject = writeList[j].GetValue(dataDT[i], null);
                    string pv = Convert.ToString(pvOnject);
                    if (pv == null)
                        pv = "";
                    this[curMaxRow + i + 2, 1 + j] = pv;
                }
            }
        }

        public void AddData(List<object> dataDT, Type type, int beginCol, List<string> notWriteCols = null)
        {
            if (dataDT == null)
            {
                throw new Exception("需要添加的表数据为空");
            }
            int curMaxRow;

            curMaxRow = this.rowAL.Count;
            IEnumerable<PropertyInfo> plist = type.GetRuntimeProperties();
            List<PropertyInfo> writeList = new List<PropertyInfo>();
            if (notWriteCols != null)
            {
                foreach (var p in plist)
                {
                    if (notWriteCols.Contains(p.Name))
                        continue;
                    writeList.Add(p);
                }
            }
            else
                writeList = plist.ToList();
            //添加标题
            for (int c = 1; c <= writeList.Count; c++)
            {
                this[1, c] = writeList[c - 1].Name;
            }
            //添加数据
            for (int i = 0; i < dataDT.Count; i++)
            {
                for (int j = 0; j < writeList.Count; j++)
                {
                    object pvOnject = writeList[j].GetValue(dataDT[i], null);
                    string pv = Convert.ToString(pvOnject);
                    if (pv == null)
                        pv = "";
                    this[curMaxRow + i + 2, beginCol + j] = pv;
                }
            }
        }

        /// <summary>
        /// 保存数据,如果当前硬盘中已经存在文件名一样的文件，将会覆盖
        /// </summary>
        public void Save()
        {
            //对数据的有效性进行判断
            if (this.fileName == null)
            {
                throw new Exception("缺少文件名");
            }
            else if (File.Exists(this.fileName))
            {
                File.Delete(this.fileName);
            }
            if (this.encoding == null)
            {
                this.encoding = Encoding.UTF8;
            }

            FileStream fsw = new FileStream(this.fileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fsw);
            for (int i = 0; i < this.rowAL.Count; i++)
            {
                sw.WriteLine(ConvertToSaveLine((List<object>)this.rowAL[i]));
            }
            sw.Dispose();
            fsw.Dispose();
        }

        /// <summary>
        /// 保存数据,如果当前硬盘中已经存在文件名一样的文件，将会覆盖
        /// </summary>
        /// <param name="fileName">文件名,包括文件路径</param>
        public void Save(string fileName)
        {
            this.fileName = fileName;
            Save();
        }

        /// <summary>
        /// 保存数据,如果当前硬盘中已经存在文件名一样的文件，将会覆盖
        /// </summary>
        /// <param name="fileName">文件名,包括文件路径</param>
        /// <param name="encoding">文件编码</param>
        public void Save(string fileName, Encoding encoding)
        {
            this.fileName = fileName;
            this.encoding = encoding;
            Save();
        }

        /// <summary>
        /// 转换成保存行
        /// </summary>
        /// <param name="colAL">一行</param>
        /// <returns></returns>
        private string ConvertToSaveLine(List<object> colAL)
        {
            string saveLine;

            saveLine = "";
            for (int i = 0; i < colAL.Count; i++)
            {
                saveLine += ConvertToSaveCell(colAL[i].ToString());
                //格子间以逗号分割
                if (i < colAL.Count - 1)
                {
                    saveLine += ",";
                }
            }

            return saveLine;
        }

        /// <summary>
        /// 字符串转换成CSV中的格子
        /// 双引号转换成两个双引号，然后首尾各加一个双引号
        /// 这样就不需要考虑逗号及换行的问题
        /// </summary>
        /// <param name="cell">格子内容</param>
        /// <returns></returns>
        private string ConvertToSaveCell(string cell)
        {
            cell = cell.Replace("\"", "\"\"");

            return "\"" + cell + "\"";
        }
    }
}