#region License
//License: New BSD License (BSD) http://www.opensource.org/licenses/BSD-3-Clause
//Project Home: https://baseservices.kilnhg.com/
//Credits: http://commonlibrarynet.codeplex.com/
//The source code of the from CommonLibrary.NET has been modified for use in
//BaseServices. Logging has been changed and some restructuring has been done. 
//Copyright (c) <trondr@outlook.com> 2012
//All rights reserved.
#endregion

#region License
/*
 * Author: Kishore Reddy
 * Url: http://commonlibrarynet.codeplex.com/
 * Title: CommonLibrary.NET
 * Copyright: � 2009 Kishore Reddy
 * License: Please refer to site for license
 * LicenseUrl: http://commonlibrarynet.codeplex.com/license
 * Description: A C# based .NET 3.5 Open-Source collection of reusable components.
 * Usage: Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 
 * Copyright (c) 2008-2011 Kishore Reddy
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */

#endregion

using System;
using System.Diagnostics;
using System.Reflection;
using Common.Logging;

namespace SccmTools.Library.Infrastructure
{
    /// <summary>
    /// Class to represent "TODO" blocks which need some action. Either "optimzation", "implementation", "bugfix", "workaround".
    /// Logging of todos are done if loglevel is 'Debug'.
    /// </summary>
    public class ToDo
    {

        #region Properties and fields
        /// <summary>
        /// Get log service
        /// </summary>
        private static ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    var stackFrame = new StackFrame(2);
                    MethodBase method = stackFrame.GetMethod();
                    if (method != null && method.DeclaringType != null)
                    {
                        _logger = LogManager.GetLogger(method.DeclaringType);
                    }
                    else
                    {
                        _logger = LogManager.GetLogger(typeof(ToDo));
                    }
                }
                return _logger;
            }
        }
        private static ILog _logger;

        /// <summary>
        /// Check if to is enabled
        /// </summary>
        public static bool IsToDoEnabled
        {
            get { return Logger.IsDebugEnabled; }
        }
        #endregion

        #region Declarations
#if NET2_0
      /// <summary>
      /// Action delegate
      /// </summary>
      public delegate void Action();
#endif
        #endregion
        /// <summary>
        /// Logs the specified action for optimization.
        /// </summary>
        /// <param name="toDoPriority">The priority.</param>
        /// <param name="author">The author.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        public static void Optimize(ToDoPriority toDoPriority, string author, string description, Action action = null)
        {
            Do("Optimization Needed", toDoPriority, author, description, action);
        }


        /// <summary>
        /// Logs the specified action as a code review
        /// </summary>
        /// <param name="toDoPriority">The priority.</param>
        /// <param name="author">The author.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        public static void CodeReview(ToDoPriority toDoPriority, string author, string description, Action action = null)
        {
            Do("CodeReview Needed", toDoPriority, author, description, action);
        }


        /// <summary>
        /// Logs the specified action as an area for a bugfix
        /// </summary>
        /// <param name="toDoPriority">The priority.</param>
        /// <param name="author">The author.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        public static void BugFix(ToDoPriority toDoPriority, string author, string description, Action action = null)
        {
            Do("BugFix Needed", toDoPriority, author, description, action);
        }


        /// <summary>
        /// Logs the specified action as a workaround
        /// </summary>
        /// <param name="toDoPriority">The priority.</param>
        /// <param name="author">The author.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        public static void WorkAround(ToDoPriority toDoPriority, string author, string description, Action action = null)
        {
            Do("Workaround performed", toDoPriority, author, description, action);
        }


        /// <summary>
        /// Logs the specified action as an area for refactoring.
        /// </summary>
        /// <param name="toDoPriority">The priority.</param>
        /// <param name="author">The author.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        public static void Refactor(ToDoPriority toDoPriority, string author, string description, Action action = null)
        {
            Do("Refactor Needed", toDoPriority, author, description, action);
        }


        /// <summary>
        /// Implementation the specified action.
        /// </summary>
        /// <param name="toDoPriority">The priority.</param>
        /// <param name="author">The author.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        public static void Implement(ToDoPriority toDoPriority, string author, string description, Action action = null)
        {
            Do("Implementation Needed", toDoPriority, author, description, action);
        }

        /// <summary>
        /// Does the specified action while logging contextual information.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="toDoPriority">The priority.</param>
        /// <param name="author">The author.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        private static void Do(string tag, ToDoPriority toDoPriority, string author, string description, Action action = null)
        {
            if (ToDo.IsToDoEnabled || toDoPriority == ToDoPriority.Critical || toDoPriority == ToDoPriority.High)
            {
                var stackTrace = new StackTrace();
                string methodName = stackTrace.GetFrame(2).GetMethod().Name;
                Type declaringType = stackTrace.GetFrame(2).GetMethod().DeclaringType;
                if (declaringType != null)
                {
                    string className = declaringType.FullName;
                    int lineNumber = stackTrace.GetFrame(2).GetFileLineNumber();
                    string fileName = stackTrace.GetFrame(2).GetFileName();
                    const string format = "{1} - Priority: {2}, Author: {3}, Description: {4}{0}At {5}.{6}, File: {7}, Line: {8}{0}";
                    string message = string.Format(format, Environment.NewLine, tag, toDoPriority, author, description, className, methodName, fileName, lineNumber);
                    Logger.Warn(message);
                }
            }
            // Now Run the action.
            if (action != null)
            {
                action();
            }
        }
    }
}
