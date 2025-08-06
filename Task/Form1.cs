using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections.Generic;
using System.Drawing; 
using System.Linq;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using System.Linq;  
using Microsoft.Msagl.Core.Geometry;
using System.Drawing.Drawing2D;  


namespace Task
{
    public partial class Form1 : Form
    {
        private GViewer _viewer = new GViewer();
        private Panel _overlayPanel = new Panel();
        private RichTextBox _customTooltip;

        public Form1(List<TableInfo> tables, List<RelationInfo> relations)
        {
            InitializeComponent();
            _customTooltip = new RichTextBox();
            _customTooltip.ReadOnly = true;
            _customTooltip.BorderStyle = BorderStyle.None;
            _customTooltip.BackColor = Color.Moccasin; // רקע כתום בהיר
            _customTooltip.ForeColor = Color.DarkBlue;
            _customTooltip.Font = new Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            _customTooltip.Visible = false;
            _customTooltip.ScrollBars = RichTextBoxScrollBars.None;
            _customTooltip.Multiline = true;
            _customTooltip.Size = new Size(200, 60); // אפשר לשנות לפי הצורך
            _customTooltip.Cursor = Cursors.Default;
            this.Controls.Add(_customTooltip);
            BuildGraph(tables, relations);
            _viewer.Paint += Viewer_Paint;

        }

        private void BuildGraph(List<TableInfo> tables, List<RelationInfo> relations)
        {
            var graph = new Microsoft.Msagl.Drawing.Graph();

            // הוספת צמתים (Nodes)
            foreach (var table in tables)
            {
                string pkText = table.PrimaryKeys != null && table.PrimaryKeys.Any()
            ? string.Join("\n", table.PrimaryKeys) // מפתחות ראשיים בשורות נפרדות
            : "";

                // כאן תוסיף גם את מפתחות הזרים
                string fkText = table.ForeignKeys != null && table.ForeignKeys.Any()
                    ? string.Join("\n", table.ForeignKeys)
                    : "";

                string label = $"{table.TableName}\n{pkText}\n{fkText}";

               // string label = $"{table.TableName}\n{pkText}";

                var node = graph.AddNode(table.TableName);
                node.LabelText = label;

                node.Attr.XRadius = 30;
                node.Attr.YRadius = 10;


                // ביטול מסגרת
                node.Attr.Color = Microsoft.Msagl.Drawing.Color.Transparent;
                node.Attr.LineWidth = 0;
                node.Attr.LabelMargin = 2;  // רווח קטן לטקסט

                // צבע רקע שקוף
                node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Transparent;
                node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Box;

                // עיצוב טקסט
                node.Label.FontName = "Segoe UI";
                node.Label.FontSize = 10;
                node.Label.FontColor = Microsoft.Msagl.Drawing.Color.DarkBlue;

                node.UserData = $"טבלה: {table.TableName}";
            }

            // הוספת קשרים (Edges)
            //foreach (var rel in relations)
            //{
            //    var edge = graph.AddEdge(rel.PrimaryTable, rel.ForeignTable);

            //    // ביטול חץ
            //    edge.Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.None;

            //    // צבע תכלת בהיר
            //    edge.Attr.Color = new Microsoft.Msagl.Drawing.Color(200, 173, 216, 230);

            //    // קו דק מאוד
            //    edge.Attr.LineWidth = 0.75;

            //    // ביטול טקסט ברירת מחדל
            //    edge.LabelText = "";

            //    // Tooltip
            //    string description = "";
            //    if (rel.Cardinality == "1:N" || rel.Cardinality == "1:n")
            //        description = $"Each {rel.PrimaryTable} may have multiple {rel.ForeignTable.ToLower()}s";
            //    else if (rel.Cardinality == "1:1")
            //        description = $"Each {rel.PrimaryTable} has exactly one {rel.ForeignTable.ToLower()}";
            //    else if (rel.Cardinality == "0:1")
            //        description = $"Each {rel.ForeignTable} may have one {rel.PrimaryTable.ToLower()}";

            //    edge.UserData = description;
            //}
            foreach (var rel in relations)
            {
                var edge = graph.AddEdge(rel.PrimaryTable, rel.ForeignTable);

                // ביטול חץ
                edge.Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.None;

                // צבע תכלת בהיר
                edge.Attr.Color = new Microsoft.Msagl.Drawing.Color(200, 173, 216, 230);

                // קו דק מאוד
                edge.Attr.LineWidth = 0.75;

                // ביטול טקסט ברירת מחדל
                edge.LabelText = "";

                // בניית תיאור מלא לכל קרדינליות אפשרית
                string description;

                if (rel.Cardinality.Equals("1:n", StringComparison.OrdinalIgnoreCase))
                {
                    description = $"Each {rel.PrimaryTable} may have multiple {rel.ForeignTable.ToLower()}s";
                }
                else if (rel.Cardinality.Equals("0:n", StringComparison.OrdinalIgnoreCase))
                {
                    description = $"Each {rel.PrimaryTable} may have zero or multiple {rel.ForeignTable.ToLower()}s";
                }
                else if (rel.Cardinality.Equals("1:1", StringComparison.OrdinalIgnoreCase))
                {
                    description = $"Each {rel.PrimaryTable} has exactly one {rel.ForeignTable.ToLower()}";
                }
                else if (rel.Cardinality.Equals("0:1", StringComparison.OrdinalIgnoreCase))
                {
                    description = $"Each {rel.ForeignTable} may have one {rel.PrimaryTable.ToLower()}";
                }
                else
                {
                    description = $"{rel.PrimaryTable} related to {rel.ForeignTable}";
                }

                //description = rel.Cardinality == "0:n" || rel.Cardinality == "0:N"
                //? $"Each {rel.PrimaryTable} may have zero or multiple {rel.ForeignTable.ToLower()}s"
                //: $"Each {rel.PrimaryTable} may have multiple {rel.ForeignTable.ToLower()}s";

                //if (rel.Cardinality == "1:N" || rel.Cardinality == "1:n")
                //{
                //    description = $"Each {rel.PrimaryTable} may have multiple {rel.ForeignTable.ToLower()}s";
                //}
                //else if (rel.Cardinality == "1:1")
                //{
                //    description = $"Each {rel.PrimaryTable} has exactly one {rel.ForeignTable.ToLower()}";
                //}
                //else if (rel.Cardinality == "0:1")
                //{
                //    description = $"Each {rel.ForeignTable} may have one {rel.PrimaryTable.ToLower()}";
                //}
                //else if (rel.Cardinality == "N:1")
                //{
                //    description = $"Each {rel.ForeignTable} may be linked to multiple {rel.PrimaryTable.ToLower()}s";
                //}
                //else if (rel.Cardinality == "0:N" || rel.Cardinality == "0:n")
                //{
                //    description = $"Each {rel.PrimaryTable} may have zero or multiple {rel.ForeignTable.ToLower()}s";
                //}
                //else
                //{
                //    description = $"{rel.PrimaryTable} related to {rel.ForeignTable}";
                //}


                edge.UserData = description;
            }

            _viewer.Graph = graph;
            _viewer.Dock = DockStyle.Fill;

            if (!this.Controls.Contains(_viewer))
                this.Controls.Add(_viewer);

            _viewer.MouseMove -= Viewer_MouseMove;
            _viewer.MouseMove += Viewer_MouseMove;
            _viewer.Paint -= Viewer_Paint;
            _viewer.Paint += Viewer_Paint;
        }

        // פונקציה ליצירת חצי מלבן עם פינות מעוגלות רק בצדדים חיצוניים
        // isTop - אם true => החצי העליון עם עיגול בפינות עליונות, אחרת החצי התחתון עם עיגול בפינות תחתונות
        private void Viewer_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;
            var viewer = sender as Microsoft.Msagl.GraphViewerGdi.GViewer;

            if (viewer?.Graph == null)
                return;

            foreach (var node in viewer.Graph.Nodes)
            {
                var bb = node.BoundingBox;

                var rect = new RectangleF(
                    (float)bb.Left,
                    (float)bb.Bottom,
                    (float)bb.Width,
                    (float)bb.Height);

                var upperRect = new RectangleF(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);
                var lowerRect = new RectangleF(rect.Left, rect.Top, rect.Width, rect.Height / 2);

                using (var upperBrush = new SolidBrush(Color.FromArgb(0, 0, 139))) // כחול כהה
                    graphics.FillPath(upperBrush, RoundedHalfRect(upperRect, 8f, true));

                using (var lowerBrush = new SolidBrush(Color.LightYellow))
                    graphics.FillPath(lowerBrush, RoundedHalfRect(lowerRect, 8f, false));

                var tableName = node.LabelText?.Split('\n')[0] ?? "";
                var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                var state = graphics.Save();
                graphics.TranslateTransform(upperRect.Left + upperRect.Width / 2, upperRect.Top + upperRect.Height / 2);
                graphics.ScaleTransform(1, -1);
                using (var font = new System.Drawing.Font(familyName: "Segoe UI", 6f, System.Drawing.FontStyle.Bold))
                using (var whiteBrush = new SolidBrush(Color.White))
                {
                    graphics.DrawString(tableName, font, whiteBrush, 0, 0, sf);
                }
                graphics.Restore(state);

                string pkText = "";
                if (!string.IsNullOrEmpty(node.LabelText))
                {
                    var lines = node.LabelText.Split('\n');
                    if (lines.Length > 1)
                        pkText = string.Join("\n", lines.Skip(1));
                }

                state = graphics.Save();
                graphics.TranslateTransform(lowerRect.Left + lowerRect.Width / 2, lowerRect.Top + lowerRect.Height / 2);
                graphics.ScaleTransform(1, -1);
                using (var keysFont = new System.Drawing.Font(familyName: "Segoe UI", 4f))
                using (var blackBrush = new SolidBrush(Color.Black))
                {
                    graphics.DrawString(pkText, keysFont, blackBrush, 0, 0, sf);
                }
                graphics.Restore(state);

                using (var pen = new Pen(Color.DarkBlue, 0.3f))
                using (var path = RoundedRect(rect, 8f))
                {
                    graphics.DrawPath(pen, path);
                }
            }

            // ציור עיגולים קטנים בקצה כל קשת
            foreach (var edge in viewer.Graph.Edges)
            {
                var targetNode = edge.TargetNode;
                var sourceNode = edge.SourceNode;

                if (targetNode == null || sourceNode == null)
                    continue;

                var start = new Microsoft.Msagl.Core.Geometry.Point(
                    (sourceNode.BoundingBox.Left + sourceNode.BoundingBox.Right) / 2,
                    (sourceNode.BoundingBox.Top + sourceNode.BoundingBox.Bottom) / 2);

                var end = new Microsoft.Msagl.Core.Geometry.Point(
                    (targetNode.BoundingBox.Left + targetNode.BoundingBox.Right) / 2,
                    (targetNode.BoundingBox.Top + targetNode.BoundingBox.Bottom) / 2);

                var vec = new Microsoft.Msagl.Core.Geometry.Point(end.X - start.X, end.Y - start.Y);
                var length = Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
                if (length == 0)
                    continue;
                var unitVec = new Microsoft.Msagl.Core.Geometry.Point(vec.X / length, vec.Y / length);

                var rectNode = targetNode.BoundingBox;

                var rectTopLeft = new Microsoft.Msagl.Core.Geometry.Point(rectNode.Left, rectNode.Top);
                var rectBottomRight = new Microsoft.Msagl.Core.Geometry.Point(rectNode.Right, rectNode.Bottom);

                Microsoft.Msagl.Core.Geometry.Point IntersectionWithRect(Microsoft.Msagl.Core.Geometry.Point rectTL,
                                                                          Microsoft.Msagl.Core.Geometry.Point rectBR,
                                                                          Microsoft.Msagl.Core.Geometry.Point lineStart,
                                                                          Microsoft.Msagl.Core.Geometry.Point lineEnd)
                {
                    var points = new List<Microsoft.Msagl.Core.Geometry.Point>();

                    // top edge
                    double y = rectTL.Y;
                    if (lineEnd.Y != lineStart.Y)
                    {
                        double x = lineStart.X + (y - lineStart.Y) * (lineEnd.X - lineStart.X) / (lineEnd.Y - lineStart.Y);
                        if (x >= rectTL.X && x <= rectBR.X)
                            points.Add(new Microsoft.Msagl.Core.Geometry.Point(x, y));
                    }

                    // bottom edge
                    y = rectBR.Y;
                    if (lineEnd.Y != lineStart.Y)
                    {
                        double x = lineStart.X + (y - lineStart.Y) * (lineEnd.X - lineStart.X) / (lineEnd.Y - lineStart.Y);
                        if (x >= rectTL.X && x <= rectBR.X)
                            points.Add(new Microsoft.Msagl.Core.Geometry.Point(x, y));
                    }

                    // left edge
                    double xLeft = rectTL.X;
                    if (lineEnd.X != lineStart.X)
                    {
                        double yLeft = lineStart.Y + (xLeft - lineStart.X) * (lineEnd.Y - lineStart.Y) / (lineEnd.X - lineStart.X);
                        if (yLeft >= rectBR.Y && yLeft <= rectTL.Y)
                            points.Add(new Microsoft.Msagl.Core.Geometry.Point(xLeft, yLeft));
                    }

                    // right edge
                    double xRight = rectBR.X;
                    if (lineEnd.X != lineStart.X)
                    {
                        double yRight = lineStart.Y + (xRight - lineStart.X) * (lineEnd.Y - lineStart.Y) / (lineEnd.X - lineStart.X);
                        if (yRight >= rectBR.Y && yRight <= rectTL.Y)
                            points.Add(new Microsoft.Msagl.Core.Geometry.Point(xRight, yRight));
                    }

                    if (points.Count == 0)
                        return lineEnd;

                    return points.OrderBy(p => (p.X - lineStart.X) * (p.X - lineStart.X) + (p.Y - lineStart.Y) * (p.Y - lineStart.Y)).First();
                }

                var circleCenter = IntersectionWithRect(rectTopLeft, rectBottomRight, start, end);

                float radius = 3f;
                using (var circleBrush = new SolidBrush(Color.LightSkyBlue))
                {
                    graphics.FillEllipse(
                        circleBrush,
                        (float)circleCenter.X - radius,
                        (float)circleCenter.Y - radius,
                        radius * 2,
                        radius * 2);
                }
            }
        }

        // פונקציות עזר ליצירת מלבן עם פינות מעוגלות
        private GraphicsPath RoundedHalfRect(RectangleF rect, float radius, bool isTop)
        {
            float diameter = radius * 2;
            var path = new GraphicsPath();

            if (isTop)
            {
                // בפועל זה החלק התחתון על המסך
                path.StartFigure();

                // קו עליון פנימי (שפיץ)
                path.AddLine(rect.Left, rect.Top, rect.Right, rect.Top);

                // קו ימין פנימי (שפיץ)
                path.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom - radius);

                // עיגול בפינה ימין תחתון (חיצונית)
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);

                // קו תחתון חיצוני
                path.AddLine(rect.Right - radius, rect.Bottom, rect.Left + radius, rect.Bottom);

                // עיגול בפינה שמאל תחתון (חיצונית)
                path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);

                // קו שמאל פנימי (שפיץ)
                path.AddLine(rect.Left, rect.Bottom - radius, rect.Left, rect.Top);

                path.CloseFigure();
            }
            else
            {
                // בפועל זה החלק העליון על המסך
                path.StartFigure();

                // עיגול בפינה שמאל עליון (חיצונית)
                path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);

                // קו עליון חיצוני
                path.AddLine(rect.Left + radius, rect.Top, rect.Right - radius, rect.Top);

                // עיגול בפינה ימין עליון (חיצונית)
                path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);

                // קו ימין פנימי (שפיץ)
                path.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom);

                // קו תחתון פנימי (שפיץ)
                path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);

                // קו שמאל פנימי (שפיץ)
                path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top + radius);

                path.CloseFigure();
            }

            return path;
        }

        private GraphicsPath RoundedRect(RectangleF rect, float radius)
        {
            float diameter = radius * 2;
            var path = new GraphicsPath();

            path.StartFigure();
            path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
      
        private void Viewer_MouseMove(object sender, MouseEventArgs e)
        {
            var viewer = sender as GViewer;
            if (viewer == null) return;

            var obj = viewer.GetObjectAt(e.Location);
            if (obj == null)
            {
                _customTooltip.Visible = false;
                return;
            }

            var prop = obj.GetType().GetProperty("DrawingObject");
            var drawingObject = prop?.GetValue(obj);

            if (drawingObject == null)
            {
                _customTooltip.Visible = false;
                return;
            }

            string tooltipText = "";

            if (drawingObject is Microsoft.Msagl.Drawing.Node node)
            {
                tooltipText = node.UserData?.ToString() ?? node.Id;
            }
            else if (drawingObject is Microsoft.Msagl.Drawing.Edge edge)
            {
                // פשוט מציגים את הטקסט המלא מתוך UserData בלי לפרש אותו מחדש
                tooltipText = edge.UserData?.ToString() ?? $"{edge.Source} → {edge.Target}";
            }
            else
            {
                _customTooltip.Visible = false;
                return;
            }

            var fixedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "each", "may", "have", "multiple", "has", "exactly", "one", "to", "the", "of", "a","zero", "or"
    };

            _customTooltip.Clear();
            _customTooltip.Width = 250;
            _customTooltip.WordWrap = true;

            var words = tooltipText.Split(' ');

            foreach (var word in words)
            {
                var cleanWord = word.Trim('.', ',', ';', ':');

                if (fixedWords.Contains(cleanWord))
                {
                    _customTooltip.SelectionFont = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
                    _customTooltip.SelectionColor = System.Drawing.Color.DarkBlue;
                }
                else
                {
                    _customTooltip.SelectionFont = new System.Drawing.Font("Segoe UI", 9,
                        System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
                    _customTooltip.SelectionColor = System.Drawing.Color.DarkBlue;
                }

                _customTooltip.AppendText(word + " ");
            }

            _customTooltip.Height = _customTooltip.GetPositionFromCharIndex(_customTooltip.TextLength).Y + 20;

            var screenPos = viewer.PointToScreen(new System.Drawing.Point(e.X + 15, e.Y + 15));
            _customTooltip.Location = this.PointToClient(screenPos);
            _customTooltip.Visible = true;
            _customTooltip.BringToFront();
        }

        private void CustomTooltip_Paint(object sender, PaintEventArgs e)
        {
            var label = sender as System.Windows.Forms.Label;
            if (label == null)
                return;

            var rect = label.ClientRectangle;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int radius = 8;

            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.StartFigure();
                path.AddArc(rect.Left, rect.Top, radius, radius, 180, 90);
                path.AddArc(rect.Right - radius, rect.Top, radius, radius, 270, 90);
                path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();

                using (var brush = new SolidBrush(label.BackColor))
                {
                    g.FillPath(brush, path);  // מילוי הרקע בלבד, בלי מסגרת
                }
            }

            // ציור הטקסט במרכז הלייבל
            TextRenderer.DrawText(
                g,
                label.Text,
                label.Font,
                rect,
                label.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
        
        
        #region Frozen Code 
        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    var graphics = e.Graphics;
        //    var viewer = sender as Microsoft.Msagl.GraphViewerGdi.GViewer;

        //    if (viewer?.Graph == null)
        //        return;

        //    foreach (var node in viewer.Graph.Nodes)
        //    {
        //        var bb = node.BoundingBox;

        //        var rect = new RectangleF(
        //            (float)bb.Left,
        //            (float)bb.Bottom,
        //            (float)bb.Width,
        //            (float)bb.Height);

        //        var upperRect = new RectangleF(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);
        //        var lowerRect = new RectangleF(rect.Left, rect.Top, rect.Width, rect.Height / 2);

        //        using (var upperBrush = new SolidBrush(Color.Navy))
        //            graphics.FillRectangle(upperBrush, upperRect);

        //        using (var lowerBrush = new SolidBrush(Color.LightYellow))
        //            graphics.FillRectangle(lowerBrush, lowerRect);

        //        var tableName = node.LabelText?.Split('\n')[0] ?? "";
        //        var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        //        var state = graphics.Save();
        //        graphics.TranslateTransform(upperRect.Left + upperRect.Width / 2, upperRect.Top + upperRect.Height / 2);
        //        graphics.ScaleTransform(1, -1);
        //        using (var font = new System.Drawing.Font(familyName: "Segoe UI", 10f, System.Drawing.FontStyle.Bold))

        //        using (var whiteBrush = new SolidBrush(Color.White))
        //        {
        //            graphics.DrawString(tableName, font, whiteBrush, 0, 0, sf);
        //        }
        //        graphics.Restore(state);

        //        string pkText = "";
        //        if (!string.IsNullOrEmpty(node.LabelText))
        //        {
        //            var lines = node.LabelText.Split('\n');
        //            if (lines.Length > 1)
        //                pkText = string.Join("\n", lines.Skip(1));
        //        }

        //        state = graphics.Save();
        //        graphics.TranslateTransform(lowerRect.Left + lowerRect.Width / 2, lowerRect.Top + lowerRect.Height / 2);
        //        graphics.ScaleTransform(1, -1);
        //        using (var keysFont = new System.Drawing.Font(familyName: "Segoe UI", 9f))
        //        using (var blackBrush = new SolidBrush(Color.Black))
        //        {
        //            graphics.DrawString(pkText, keysFont, blackBrush, 0, 0, sf);
        //        }
        //        graphics.Restore(state);

        //        using (var pen = new Pen(Color.DarkBlue, 1.5f))
        //        using (var path = GetRoundedRect(rect, 8))
        //        {
        //            graphics.DrawPath(pen, path);
        //        }
        //    }

        //    // ציור עיגולים קטנים בקצה כל קשת
        //    foreach (var edge in viewer.Graph.Edges)
        //    {
        //        var targetNode = edge.TargetNode;
        //        var sourceNode = edge.SourceNode;

        //        if (targetNode == null || sourceNode == null)
        //            continue;

        //        var start = new Microsoft.Msagl.Core.Geometry.Point(
        //            (sourceNode.BoundingBox.Left + sourceNode.BoundingBox.Right) / 2,
        //            (sourceNode.BoundingBox.Top + sourceNode.BoundingBox.Bottom) / 2);

        //        var end = new Microsoft.Msagl.Core.Geometry.Point(
        //            (targetNode.BoundingBox.Left + targetNode.BoundingBox.Right) / 2,
        //            (targetNode.BoundingBox.Top + targetNode.BoundingBox.Bottom) / 2);

        //        var vec = new Microsoft.Msagl.Core.Geometry.Point(end.X - start.X, end.Y - start.Y);
        //        var length = Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        //        if (length == 0)
        //            continue;
        //        var unitVec = new Microsoft.Msagl.Core.Geometry.Point(vec.X / length, vec.Y / length);

        //        var rectNode = targetNode.BoundingBox;

        //        var rectTopLeft = new Microsoft.Msagl.Core.Geometry.Point(rectNode.Left, rectNode.Top);
        //        var rectBottomRight = new Microsoft.Msagl.Core.Geometry.Point(rectNode.Right, rectNode.Bottom);

        //        Microsoft.Msagl.Core.Geometry.Point IntersectionWithRect(Microsoft.Msagl.Core.Geometry.Point rectTL,
        //                                                                  Microsoft.Msagl.Core.Geometry.Point rectBR,
        //                                                                  Microsoft.Msagl.Core.Geometry.Point lineStart,
        //                                                                  Microsoft.Msagl.Core.Geometry.Point lineEnd)
        //        {
        //            var points = new List<Microsoft.Msagl.Core.Geometry.Point>();

        //            // top edge
        //            double y = rectTL.Y;
        //            if (lineEnd.Y != lineStart.Y)
        //            {
        //                double x = lineStart.X + (y - lineStart.Y) * (lineEnd.X - lineStart.X) / (lineEnd.Y - lineStart.Y);
        //                if (x >= rectTL.X && x <= rectBR.X)
        //                    points.Add(new Microsoft.Msagl.Core.Geometry.Point(x, y));
        //            }

        //            // bottom edge
        //            y = rectBR.Y;
        //            if (lineEnd.Y != lineStart.Y)
        //            {
        //                double x = lineStart.X + (y - lineStart.Y) * (lineEnd.X - lineStart.X) / (lineEnd.Y - lineStart.Y);
        //                if (x >= rectTL.X && x <= rectBR.X)
        //                    points.Add(new Microsoft.Msagl.Core.Geometry.Point(x, y));
        //            }

        //            // left edge
        //            double xLeft = rectTL.X;
        //            if (lineEnd.X != lineStart.X)
        //            {
        //                double yLeft = lineStart.Y + (xLeft - lineStart.X) * (lineEnd.Y - lineStart.Y) / (lineEnd.X - lineStart.X);
        //                if (yLeft >= rectBR.Y && yLeft <= rectTL.Y)
        //                    points.Add(new Microsoft.Msagl.Core.Geometry.Point(xLeft, yLeft));
        //            }

        //            // right edge
        //            double xRight = rectBR.X;
        //            if (lineEnd.X != lineStart.X)
        //            {
        //                double yRight = lineStart.Y + (xRight - lineStart.X) * (lineEnd.Y - lineStart.Y) / (lineEnd.X - lineStart.X);
        //                if (yRight >= rectBR.Y && yRight <= rectTL.Y)
        //                    points.Add(new Microsoft.Msagl.Core.Geometry.Point(xRight, yRight));
        //            }

        //            if (points.Count == 0)
        //                return lineEnd;

        //            return points.OrderBy(p => (p.X - lineStart.X) * (p.X - lineStart.X) + (p.Y - lineStart.Y) * (p.Y - lineStart.Y)).First();
        //        }

        //        var circleCenter = IntersectionWithRect(rectTopLeft, rectBottomRight, start, end);

        //        float radius = 3f;
        //        using (var circleBrush = new SolidBrush(Color.LightSkyBlue))
        //        {
        //            graphics.FillEllipse(
        //                circleBrush,
        //                (float)circleCenter.X - radius,
        //                (float)circleCenter.Y - radius,
        //                radius * 2,
        //                radius * 2);
        //        }
        //    }
        //}

        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    var graphics = e.Graphics;
        //    var viewer = sender as Microsoft.Msagl.GraphViewerGdi.GViewer;

        //    if (viewer?.Graph == null)
        //        return;

        //    // ציור הצמתים (ריבועים כחול/קרם עם טקסט)
        //    foreach (var node in viewer.Graph.Nodes)
        //    {
        //        var bb = node.BoundingBox;

        //        var rect = new System.Drawing.RectangleF(
        //            (float)bb.Left,
        //            (float)bb.Bottom,
        //            (float)bb.Width,
        //            (float)bb.Height);

        //        var upperRect = new System.Drawing.RectangleF(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);
        //        var lowerRect = new System.Drawing.RectangleF(rect.Left, rect.Top, rect.Width, rect.Height / 2);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkBlue))
        //            graphics.FillRectangle(brush, upperRect);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Bisque))
        //            graphics.FillRectangle(brush, lowerRect);

        //        var tableName = node.LabelText?.Split('\n')[0] ?? "";
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(upperRect.Left + upperRect.Width / 2, upperRect.Top + upperRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(tableName, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        string pkText = "";
        //        if (!string.IsNullOrEmpty(node.LabelText))
        //        {
        //            var lines = node.LabelText.Split('\n');
        //            if (lines.Length > 1)
        //                pkText = string.Join("\n", lines.Skip(1));
        //        }
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 9))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(lowerRect.Left + lowerRect.Width / 2, lowerRect.Top + lowerRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(pkText, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        using (var pen = new System.Drawing.Pen(System.Drawing.Color.DarkBlue, 1.5f))
        //            graphics.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        //    }

        //    // ציור עיגולים קטנים בקצה כל קשת, מבלי להשתמש ב Curve או EdgeGeometry
        //    foreach (var edge in viewer.Graph.Edges)
        //    {
        //        var targetNode = edge.TargetNode;
        //        var sourceNode = edge.SourceNode;

        //        if (targetNode == null || sourceNode == null)
        //            continue;

        //        var bb = targetNode.BoundingBox;

        //        // מרכז הנוד היעד
        //        var targetCenter = new Microsoft.Msagl.Core.Geometry.Point(
        //            (bb.Left + bb.Right) / 2,
        //            (bb.Top + bb.Bottom) / 2);

        //        // מרכז הנוד המקור
        //        var sourceCenter = new Microsoft.Msagl.Core.Geometry.Point(
        //            (sourceNode.BoundingBox.Left + sourceNode.BoundingBox.Right) / 2,
        //            (sourceNode.BoundingBox.Top + sourceNode.BoundingBox.Bottom) / 2);

        //        // וקטור מהמקור ליעד
        //        var vec = new Microsoft.Msagl.Core.Geometry.Point(targetCenter.X - sourceCenter.X, targetCenter.Y - sourceCenter.Y);

        //        double length = Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        //        if (length == 0) continue;
        //        var unitVec = new Microsoft.Msagl.Core.Geometry.Point(vec.X / length, vec.Y / length);

        //        // חישוב מרחק ההיסט מהקצה: נניח שהנוד הוא מלבן, נחשב את חצי רוחב/גובה וניקח את המרחק המקסימלי לכיוון הוקטור
        //        double halfWidth = (bb.Right - bb.Left) / 2;
        //        double halfHeight = (bb.Top - bb.Bottom) / 2;

        //        // כיוון הוקטור באיקס ואי-איקס חיובי/שלילי משפיע על המיקום בקצה (הערכה גסה)
        //        double offset = 0;
        //        if (Math.Abs(unitVec.X) > Math.Abs(unitVec.Y))
        //            offset = halfWidth / Math.Abs(unitVec.X);
        //        else
        //            offset = halfHeight / Math.Abs(unitVec.Y);

        //        // מיקום העיגול מחוץ לנוד, קצת יותר מ-offset, נניח 3 פיקסלים עוד
        //        double distanceFromCenter = offset + 3;

        //        var circleCenter = new Microsoft.Msagl.Core.Geometry.Point(
        //            targetCenter.X - unitVec.X * distanceFromCenter,
        //            targetCenter.Y - unitVec.Y * distanceFromCenter);

        //        float radius = 2.5f; // הקטןתי את הרדיוס
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.LightSkyBlue))
        //        {
        //            graphics.FillEllipse(
        //                brush,
        //                (float)circleCenter.X - radius,
        //                (float)circleCenter.Y - radius,
        //                radius * 2,
        //                radius * 2
        //            );
        //        }
        //    }
        //}


        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    var graphics = e.Graphics;
        //    var viewer = sender as Microsoft.Msagl.GraphViewerGdi.GViewer;

        //    if (viewer?.Graph == null)
        //        return;

        //    foreach (var node in viewer.Graph.Nodes)
        //    {
        //        var bb = node.BoundingBox;

        //        var rect = new System.Drawing.RectangleF(
        //            (float)bb.Left,
        //            (float)bb.Bottom,
        //            (float)bb.Width,
        //            (float)bb.Height);

        //        // החלפת המלבנים - upperRect למעלה, lowerRect למטה (לפי כיוון Y הפוך)
        //        var upperRect = new System.Drawing.RectangleF(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);
        //        var lowerRect = new System.Drawing.RectangleF(rect.Left, rect.Top, rect.Width, rect.Height / 2);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkBlue))
        //            graphics.FillRectangle(brush, upperRect);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Bisque))
        //            graphics.FillRectangle(brush, lowerRect);

        //        var tableName = node.LabelText?.Split('\n')[0] ?? "";
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(upperRect.Left + upperRect.Width / 2, upperRect.Top + upperRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(tableName, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        string pkText = "";
        //        if (!string.IsNullOrEmpty(node.LabelText))
        //        {
        //            var lines = node.LabelText.Split('\n');
        //            if (lines.Length > 1)
        //                pkText = string.Join("\n", lines.Skip(1));
        //        }
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 9))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(lowerRect.Left + lowerRect.Width / 2, lowerRect.Top + lowerRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(pkText, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        using (var pen = new System.Drawing.Pen(System.Drawing.Color.DarkBlue, 1.5f))
        //            graphics.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        //    }
        //}




        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    var graphics = e.Graphics;
        //    var viewer = sender as Microsoft.Msagl.GraphViewerGdi.GViewer;

        //    if (viewer?.Graph == null)
        //        return;

        //    foreach (var node in viewer.Graph.Nodes)
        //    {
        //        var bb = node.BoundingBox;

        //        var rect = new System.Drawing.RectangleF(
        //            (float)bb.Left,
        //            (float)bb.Bottom,
        //            (float)bb.Width,
        //            (float)bb.Height);

        //        // החלפת המלבנים - upperRect למעלה, lowerRect למטה (לפי כיוון Y הפוך)
        //        var upperRect = new System.Drawing.RectangleF(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);
        //        var lowerRect = new System.Drawing.RectangleF(rect.Left, rect.Top, rect.Width, rect.Height / 2);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkBlue))
        //            graphics.FillRectangle(brush, upperRect);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Bisque))
        //            graphics.FillRectangle(brush, lowerRect);

        //        var tableName = node.LabelText?.Split('\n')[0] ?? "";
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(upperRect.Left + upperRect.Width / 2, upperRect.Top + upperRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(tableName, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        string pkText = "";
        //        if (!string.IsNullOrEmpty(node.LabelText))
        //        {
        //            var lines = node.LabelText.Split('\n');
        //            if (lines.Length > 1)
        //                pkText = string.Join("\n", lines.Skip(1));
        //        }
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 9))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(lowerRect.Left + lowerRect.Width / 2, lowerRect.Top + lowerRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(pkText, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        using (var pen = new System.Drawing.Pen(System.Drawing.Color.DarkBlue, 1.5f))
        //            graphics.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        //    }
        //}



        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    var graphics = e.Graphics;
        //    var viewer = sender as Microsoft.Msagl.GraphViewerGdi.GViewer;

        //    if (viewer?.Graph == null)
        //        return;

        //    foreach (var node in viewer.Graph.Nodes)
        //    {
        //        // נניח שכל Node מחולק לשני חלקים:
        //        // חצי עליון = שם הטבלה (כחול כהה)
        //        // חצי תחתון = המפתח הראשי (קרם)

        //        // נשאב מיקום וגודל הצומת
        //        var boundingBox = node.BoundingBox;
        //        var rect = new System.Drawing.RectangleF(
        //            (float)boundingBox.Left,
        //            (float)boundingBox.Bottom,
        //            (float)boundingBox.Width,
        //            (float)boundingBox.Height);

        //        // מחלקים את המלבן לשני חלקים אנכית
        //        var upperRect = new System.Drawing.RectangleF(rect.Left, rect.Top, rect.Width, rect.Height / 2);
        //        var lowerRect = new System.Drawing.RectangleF(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);

        //        // צביעה של החלק העליון - כחול כהה
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkBlue))
        //        {
        //            graphics.FillRectangle(brush, upperRect);
        //        }

        //        // צביעה של החלק התחתון - קרם (Bisque)
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Bisque))
        //        {
        //            graphics.FillRectangle(brush, lowerRect);
        //        }

        //        // רישום טקסטים:

        //        // טקסט עליון - שם הטבלה
        //        var tableName = node.LabelText?.Split('\n')[0] ?? "";

        //        using (var sf = new System.Drawing.StringFormat()
        //        {
        //            Alignment = System.Drawing.StringAlignment.Center,
        //            LineAlignment = System.Drawing.StringAlignment.Center
        //        })
        //        using (var font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
        //        {
        //            graphics.DrawString(tableName, font, brush, upperRect, sf);
        //        }

        //        // טקסט תחתון - מפתח ראשי (השארת שאר השורות אחרי השורה הראשונה)
        //        string pkText = "";
        //        if (!string.IsNullOrEmpty(node.LabelText))
        //        {
        //            var lines = node.LabelText.Split('\n');
        //            if (lines.Length > 1)
        //            {
        //                pkText = string.Join("\n", lines.Skip(1));
        //            }
        //        }

        //        using (var sf = new System.Drawing.StringFormat()
        //        {
        //            Alignment = System.Drawing.StringAlignment.Center,
        //            LineAlignment = System.Drawing.StringAlignment.Center
        //        })
        //        using (var font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
        //        {
        //            graphics.DrawString(pkText, font, brush, lowerRect, sf);
        //        }

        //        // ציור מסגרת סביב כל הצומת
        //        using (var pen = new System.Drawing.Pen(System.Drawing.Color.DarkBlue, 1.5f))
        //        {
        //            graphics.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        //        }
        //    }
        //}





        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    if (_viewer.Graph == null || _viewer.Graph.GeometryGraph == null)
        //        return;

        //    var g = e.Graphics;

        //    using (var brush = new SolidBrush(System.Drawing.Color.LightSkyBlue))
        //    {
        //        foreach (var edge in _viewer.Graph.Edges)
        //        {
        //            var geomEdge = edge.GeometryEdge;
        //            if (geomEdge?.Curve == null)
        //                continue;

        //            // קצה הקו (נקודת הסיום)
        //            var target = geomEdge.Curve.End;

        //            // המרה לקואורדינטות מסך
        //            PointF screenPoint = GraphPointToScreen(target);

        //            // ציור עיגול בקצה הקו
        //            float radius = 2;
        //            g.FillEllipse(
        //                brush,
        //                screenPoint.X - radius,
        //                screenPoint.Y - radius,
        //                radius * 2,
        //                radius * 2
        //            );
        //        }
        //    }
        //}


        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    if (_viewer.Graph == null || _viewer.Graph.GeometryGraph == null)
        //        return;

        //    var g = e.Graphics;

        //    foreach (var node in _viewer.Graph.Nodes)
        //    {
        //        // חילוק טקסט לשורות
        //        var labelText = node.LabelText.Split(new[] { '\n' }, StringSplitOptions.None);

        //        // חישוב המיקום של הצומת
        //        var centerX = (float)node.GeometryNode.Center.X; // מרכז הצומת ב-X
        //        var centerY = (float)node.GeometryNode.Center.Y; // מרכז הצומת ב-Y

        //        // קביעת ממדי הצומת
        //        var width = 120;  // אורך הצומת
        //        var height = 60;  // גובה הצומת

        //        // קביעת מיקום הצומת
        //        var x = centerX - width / 2;
        //        var y = centerY - height / 2;

        //        // ציור הצומת עם תיקוני סיבוב
        //        using (var topBrush = new SolidBrush(System.Drawing.Color.DarkBlue)) // צבע כחול כהה לחלק העליון
        //        using (var bottomBrush = new SolidBrush(System.Drawing.Color.LemonChiffon)) // צבע בהיר לחלק התחתון
        //        {
        //            float topHeight = height / 2;  // חצי גובה לצורך החלק העליון
        //            var topRect = new RectangleF(x, y, width, topHeight);  // חלק עליון
        //            var bottomRect = new RectangleF(x, y + topHeight, width, height - topHeight); // חלק תחתון

        //            // צבעים של החלקים
        //            g.FillRectangle(topBrush, topRect);   // צבע כחול כהה בחלק העליון
        //            g.FillRectangle(bottomBrush, bottomRect);  // צבע בהיר בחלק התחתון

        //            // ציור של גבול שחור מסביב לצומת
        //            using (var pen = new Pen(System.Drawing.Color.Black, 2f)) // גבול שחור עבה
        //            {
        //                g.DrawRectangle(pen, x, y, width, height);  // גבול מסביב לצומת
        //            }

        //            // ציור טקסט בחלקים
        //            using (var font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)) // גופן לטקסט
        //            using (var textBrush = new SolidBrush(System.Drawing.Color.White)) // צבע טקסט לבן
        //            {
        //                // טקסט בחלק העליון (שם הטבלה)
        //                g.DrawString(labelText[0], font, textBrush, topRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.DirectionRightToLeft });

        //                // טקסט בחלק התחתון (מפתחות ראשיים)
        //                if (labelText.Length > 1)
        //                {
        //                    g.DrawString(labelText[1], font, textBrush, bottomRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.DirectionRightToLeft });
        //                }
        //            }
        //        }
        //    }
        //}

        //זה טוב הלמטה
        //private PointF GraphPointToScreen(Microsoft.Msagl.Core.Geometry.Point graphPoint)
        //{
        //    // המרה ישירה, אם הגרף לא עובר זום או פאן, אפשר פשוט להמיר לנקודה במסך.
        //    // כאן אנחנו מניחים שהקואורדינטות במסך וגרף חופפים (עבור רוב המקרים).

        //    // הגישה הפשוטה: פשוט החזר את הקואורדינטות כפי שהן
        //    return new PointF((float)graphPoint.X, (float)graphPoint.Y);
        //}

        //private void BuildGraph(List<TableInfo> tables, List<RelationInfo> relations)
        //{
        //    var graph = new Microsoft.Msagl.Drawing.Graph();

        //    foreach (var table in tables)
        //    {
        //        string label = table.TableName;
        //        if (table.PrimaryKeys.Count > 0)
        //            label += "\nPK: " + string.Join(", ", table.PrimaryKeys);

        //        var node = graph.AddNode(table.TableName);
        //        node.LabelText = label;

        //        // פה Microsoft.Msagl.Drawing.Color חייבים - כי זה attr של msagl
        //        node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;

        //        node.Label.FontSize = 10;
        //        node.UserData = $"טבלה: {table.TableName}";

        //        node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Box;
        //        node.Attr.LineWidth = 1.5;
        //        node.Attr.Color = Microsoft.Msagl.Drawing.Color.Black;
        //        node.Attr.XRadius = 5;
        //        node.Attr.YRadius = 5;
        //        node.Label.FontName = "Segoe UI";
        //    }

        //    foreach (var rel in relations)
        //    {
        //        var edge = graph.AddEdge(rel.PrimaryTable, rel.ForeignTable);
        //        edge.Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.Normal;
        //        edge.LabelText = rel.Cardinality;

        //        string description = "";
        //        if (rel.Cardinality == "1:N" || rel.Cardinality == "1:n")
        //            description = $"Each {rel.PrimaryTable} may have multiple {rel.ForeignTable.ToLower()}s";
        //        else if (rel.Cardinality == "1:1")
        //            description = $"Each {rel.PrimaryTable} has exactly one {rel.ForeignTable.ToLower()}";
        //        else if (rel.Cardinality == "0:1")
        //            description = $"Each {rel.ForeignTable} may have one {rel.PrimaryTable.ToLower()}";

        //        edge.UserData = description;
        //    }

        //    _viewer.Graph = graph;
        //    _viewer.Dock = DockStyle.Fill;

        //    if (!this.Controls.Contains(_viewer))
        //        this.Controls.Add(_viewer);

        //    _viewer.MouseMove -= Viewer_MouseMove;
        //    _viewer.MouseMove += Viewer_MouseMove;
        //}
        /*        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    var graphics = e.Graphics;
        //    var viewer = sender as Microsoft.Msagl.GraphViewerGdi.GViewer;

        //    if (viewer?.Graph == null)
        //        return;

        //    foreach (var node in viewer.Graph.Nodes)
        //    {
        //        var bb = node.BoundingBox;

        //        var rect = new RectangleF(
        //            (float)bb.Left,
        //            (float)bb.Bottom,
        //            (float)bb.Width,
        //            (float)bb.Height);

        //        var upperRect = new RectangleF(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);
        //        var lowerRect = new RectangleF(rect.Left, rect.Top, rect.Width, rect.Height / 2);

        //        using (var upperBrush = new SolidBrush(Color.Navy))
        //            graphics.FillRectangle(upperBrush, upperRect);

        //        using (var lowerBrush = new SolidBrush(Color.LightYellow))
        //            graphics.FillRectangle(lowerBrush, lowerRect);

        //        var tableName = node.LabelText?.Split('\n')[0] ?? "";
        //        var sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        //        var state = graphics.Save();
        //        graphics.TranslateTransform(upperRect.Left + upperRect.Width / 2, upperRect.Top + upperRect.Height / 2);
        //        graphics.ScaleTransform(1, -1);
        //        using (var font = new System.Drawing.Font(familyName: "Segoe UI", 10f, System.Drawing.FontStyle.Bold))

        //        using (var whiteBrush = new SolidBrush(Color.White))
        //        {
        //            graphics.DrawString(tableName, font, whiteBrush, 0, 0, sf);
        //        }
        //        graphics.Restore(state);

        //        string pkText = "";
        //        if (!string.IsNullOrEmpty(node.LabelText))
        //        {
        //            var lines = node.LabelText.Split('\n');
        //            if (lines.Length > 1)
        //                pkText = string.Join("\n", lines.Skip(1));
        //        }

        //        state = graphics.Save();
        //        graphics.TranslateTransform(lowerRect.Left + lowerRect.Width / 2, lowerRect.Top + lowerRect.Height / 2);
        //        graphics.ScaleTransform(1, -1);
        //        using (var keysFont = new System.Drawing.Font(familyName: "Segoe UI", 9f))
        //        using (var blackBrush = new SolidBrush(Color.Black))
        //        {
        //            graphics.DrawString(pkText, keysFont, blackBrush, 0, 0, sf);
        //        }
        //        graphics.Restore(state);

        //        using (var pen = new Pen(Color.DarkBlue, 1.5f))
        //        using (var path = GetRoundedRect(rect, 8))
        //        {
        //            graphics.DrawPath(pen, path);
        //        }
        //    }

        //    // ציור עיגולים קטנים בקצה כל קשת
        //    foreach (var edge in viewer.Graph.Edges)
        //    {
        //        var targetNode = edge.TargetNode;
        //        var sourceNode = edge.SourceNode;

        //        if (targetNode == null || sourceNode == null)
        //            continue;

        //        var start = new Microsoft.Msagl.Core.Geometry.Point(
        //            (sourceNode.BoundingBox.Left + sourceNode.BoundingBox.Right) / 2,
        //            (sourceNode.BoundingBox.Top + sourceNode.BoundingBox.Bottom) / 2);

        //        var end = new Microsoft.Msagl.Core.Geometry.Point(
        //            (targetNode.BoundingBox.Left + targetNode.BoundingBox.Right) / 2,
        //            (targetNode.BoundingBox.Top + targetNode.BoundingBox.Bottom) / 2);

        //        var vec = new Microsoft.Msagl.Core.Geometry.Point(end.X - start.X, end.Y - start.Y);
        //        var length = Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        //        if (length == 0)
        //            continue;
        //        var unitVec = new Microsoft.Msagl.Core.Geometry.Point(vec.X / length, vec.Y / length);

        //        var rectNode = targetNode.BoundingBox;

        //        var rectTopLeft = new Microsoft.Msagl.Core.Geometry.Point(rectNode.Left, rectNode.Top);
        //        var rectBottomRight = new Microsoft.Msagl.Core.Geometry.Point(rectNode.Right, rectNode.Bottom);

        //        Microsoft.Msagl.Core.Geometry.Point IntersectionWithRect(Microsoft.Msagl.Core.Geometry.Point rectTL,
        //                                                                  Microsoft.Msagl.Core.Geometry.Point rectBR,
        //                                                                  Microsoft.Msagl.Core.Geometry.Point lineStart,
        //                                                                  Microsoft.Msagl.Core.Geometry.Point lineEnd)
        //        {
        //            var points = new List<Microsoft.Msagl.Core.Geometry.Point>();

        //            // top edge
        //            double y = rectTL.Y;
        //            if (lineEnd.Y != lineStart.Y)
        //            {
        //                double x = lineStart.X + (y - lineStart.Y) * (lineEnd.X - lineStart.X) / (lineEnd.Y - lineStart.Y);
        //                if (x >= rectTL.X && x <= rectBR.X)
        //                    points.Add(new Microsoft.Msagl.Core.Geometry.Point(x, y));
        //            }

        //            // bottom edge
        //            y = rectBR.Y;
        //            if (lineEnd.Y != lineStart.Y)
        //            {
        //                double x = lineStart.X + (y - lineStart.Y) * (lineEnd.X - lineStart.X) / (lineEnd.Y - lineStart.Y);
        //                if (x >= rectTL.X && x <= rectBR.X)
        //                    points.Add(new Microsoft.Msagl.Core.Geometry.Point(x, y));
        //            }

        //            // left edge
        //            double xLeft = rectTL.X;
        //            if (lineEnd.X != lineStart.X)
        //            {
        //                double yLeft = lineStart.Y + (xLeft - lineStart.X) * (lineEnd.Y - lineStart.Y) / (lineEnd.X - lineStart.X);
        //                if (yLeft >= rectBR.Y && yLeft <= rectTL.Y)
        //                    points.Add(new Microsoft.Msagl.Core.Geometry.Point(xLeft, yLeft));
        //            }

        //            // right edge
        //            double xRight = rectBR.X;
        //            if (lineEnd.X != lineStart.X)
        //            {
        //                double yRight = lineStart.Y + (xRight - lineStart.X) * (lineEnd.Y - lineStart.Y) / (lineEnd.X - lineStart.X);
        //                if (yRight >= rectBR.Y && yRight <= rectTL.Y)
        //                    points.Add(new Microsoft.Msagl.Core.Geometry.Point(xRight, yRight));
        //            }

        //            if (points.Count == 0)
        //                return lineEnd;

        //            return points.OrderBy(p => (p.X - lineStart.X) * (p.X - lineStart.X) + (p.Y - lineStart.Y) * (p.Y - lineStart.Y)).First();
        //        }

        //        var circleCenter = IntersectionWithRect(rectTopLeft, rectBottomRight, start, end);

        //        float radius = 3f;
        //        using (var circleBrush = new SolidBrush(Color.LightSkyBlue))
        //        {
        //            graphics.FillEllipse(
        //                circleBrush,
        //                (float)circleCenter.X - radius,
        //                (float)circleCenter.Y - radius,
        //                radius * 2,
        //                radius * 2);
        //        }
        //    }
        //}

        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    var graphics = e.Graphics;
        //    var viewer = sender as Microsoft.Msagl.GraphViewerGdi.GViewer;

        //    if (viewer?.Graph == null)
        //        return;

        //    // ציור הצמתים (ריבועים כחול/קרם עם טקסט)
        //    foreach (var node in viewer.Graph.Nodes)
        //    {
        //        var bb = node.BoundingBox;

        //        var rect = new System.Drawing.RectangleF(
        //            (float)bb.Left,
        //            (float)bb.Bottom,
        //            (float)bb.Width,
        //            (float)bb.Height);

        //        var upperRect = new System.Drawing.RectangleF(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);
        //        var lowerRect = new System.Drawing.RectangleF(rect.Left, rect.Top, rect.Width, rect.Height / 2);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkBlue))
        //            graphics.FillRectangle(brush, upperRect);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Bisque))
        //            graphics.FillRectangle(brush, lowerRect);

        //        var tableName = node.LabelText?.Split('\n')[0] ?? "";
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(upperRect.Left + upperRect.Width / 2, upperRect.Top + upperRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(tableName, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        string pkText = "";
        //        if (!string.IsNullOrEmpty(node.LabelText))
        //        {
        //            var lines = node.LabelText.Split('\n');
        //            if (lines.Length > 1)
        //                pkText = string.Join("\n", lines.Skip(1));
        //        }
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 9))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(lowerRect.Left + lowerRect.Width / 2, lowerRect.Top + lowerRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(pkText, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        using (var pen = new System.Drawing.Pen(System.Drawing.Color.DarkBlue, 1.5f))
        //            graphics.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        //    }

        //    // ציור עיגולים קטנים בקצה כל קשת, מבלי להשתמש ב Curve או EdgeGeometry
        //    foreach (var edge in viewer.Graph.Edges)
        //    {
        //        var targetNode = edge.TargetNode;
        //        var sourceNode = edge.SourceNode;

        //        if (targetNode == null || sourceNode == null)
        //            continue;

        //        var bb = targetNode.BoundingBox;

        //        // מרכז הנוד היעד
        //        var targetCenter = new Microsoft.Msagl.Core.Geometry.Point(
        //            (bb.Left + bb.Right) / 2,
        //            (bb.Top + bb.Bottom) / 2);

        //        // מרכז הנוד המקור
        //        var sourceCenter = new Microsoft.Msagl.Core.Geometry.Point(
        //            (sourceNode.BoundingBox.Left + sourceNode.BoundingBox.Right) / 2,
        //            (sourceNode.BoundingBox.Top + sourceNode.BoundingBox.Bottom) / 2);

        //        // וקטור מהמקור ליעד
        //        var vec = new Microsoft.Msagl.Core.Geometry.Point(targetCenter.X - sourceCenter.X, targetCenter.Y - sourceCenter.Y);

        //        double length = Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
        //        if (length == 0) continue;
        //        var unitVec = new Microsoft.Msagl.Core.Geometry.Point(vec.X / length, vec.Y / length);

        //        // חישוב מרחק ההיסט מהקצה: נניח שהנוד הוא מלבן, נחשב את חצי רוחב/גובה וניקח את המרחק המקסימלי לכיוון הוקטור
        //        double halfWidth = (bb.Right - bb.Left) / 2;
        //        double halfHeight = (bb.Top - bb.Bottom) / 2;

        //        // כיוון הוקטור באיקס ואי-איקס חיובי/שלילי משפיע על המיקום בקצה (הערכה גסה)
        //        double offset = 0;
        //        if (Math.Abs(unitVec.X) > Math.Abs(unitVec.Y))
        //            offset = halfWidth / Math.Abs(unitVec.X);
        //        else
        //            offset = halfHeight / Math.Abs(unitVec.Y);

        //        // מיקום העיגול מחוץ לנוד, קצת יותר מ-offset, נניח 3 פיקסלים עוד
        //        double distanceFromCenter = offset + 3;

        //        var circleCenter = new Microsoft.Msagl.Core.Geometry.Point(
        //            targetCenter.X - unitVec.X * distanceFromCenter,
        //            targetCenter.Y - unitVec.Y * distanceFromCenter);

        //        float radius = 2.5f; // הקטןתי את הרדיוס
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.LightSkyBlue))
        //        {
        //            graphics.FillEllipse(
        //                brush,
        //                (float)circleCenter.X - radius,
        //                (float)circleCenter.Y - radius,
        //                radius * 2,
        //                radius * 2
        //            );
        //        }
        //    }
        //}


        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    var graphics = e.Graphics;
        //    var viewer = sender as Microsoft.Msagl.GraphViewerGdi.GViewer;

        //    if (viewer?.Graph == null)
        //        return;

        //    foreach (var node in viewer.Graph.Nodes)
        //    {
        //        var bb = node.BoundingBox;

        //        var rect = new System.Drawing.RectangleF(
        //            (float)bb.Left,
        //            (float)bb.Bottom,
        //            (float)bb.Width,
        //            (float)bb.Height);

        //        // החלפת המלבנים - upperRect למעלה, lowerRect למטה (לפי כיוון Y הפוך)
        //        var upperRect = new System.Drawing.RectangleF(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);
        //        var lowerRect = new System.Drawing.RectangleF(rect.Left, rect.Top, rect.Width, rect.Height / 2);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkBlue))
        //            graphics.FillRectangle(brush, upperRect);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Bisque))
        //            graphics.FillRectangle(brush, lowerRect);

        //        var tableName = node.LabelText?.Split('\n')[0] ?? "";
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(upperRect.Left + upperRect.Width / 2, upperRect.Top + upperRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(tableName, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        string pkText = "";
        //        if (!string.IsNullOrEmpty(node.LabelText))
        //        {
        //            var lines = node.LabelText.Split('\n');
        //            if (lines.Length > 1)
        //                pkText = string.Join("\n", lines.Skip(1));
        //        }
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 9))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(lowerRect.Left + lowerRect.Width / 2, lowerRect.Top + lowerRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(pkText, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        using (var pen = new System.Drawing.Pen(System.Drawing.Color.DarkBlue, 1.5f))
        //            graphics.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        //    }
        //}




        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    var graphics = e.Graphics;
        //    var viewer = sender as Microsoft.Msagl.GraphViewerGdi.GViewer;

        //    if (viewer?.Graph == null)
        //        return;

        //    foreach (var node in viewer.Graph.Nodes)
        //    {
        //        var bb = node.BoundingBox;

        //        var rect = new System.Drawing.RectangleF(
        //            (float)bb.Left,
        //            (float)bb.Bottom,
        //            (float)bb.Width,
        //            (float)bb.Height);

        //        // החלפת המלבנים - upperRect למעלה, lowerRect למטה (לפי כיוון Y הפוך)
        //        var upperRect = new System.Drawing.RectangleF(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);
        //        var lowerRect = new System.Drawing.RectangleF(rect.Left, rect.Top, rect.Width, rect.Height / 2);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkBlue))
        //            graphics.FillRectangle(brush, upperRect);

        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Bisque))
        //            graphics.FillRectangle(brush, lowerRect);

        //        var tableName = node.LabelText?.Split('\n')[0] ?? "";
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(upperRect.Left + upperRect.Width / 2, upperRect.Top + upperRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(tableName, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        string pkText = "";
        //        if (!string.IsNullOrEmpty(node.LabelText))
        //        {
        //            var lines = node.LabelText.Split('\n');
        //            if (lines.Length > 1)
        //                pkText = string.Join("\n", lines.Skip(1));
        //        }
        //        using (var sf = new System.Drawing.StringFormat() { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center })
        //        using (var font = new System.Drawing.Font("Segoe UI", 9))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
        //        {
        //            var state = graphics.Save();
        //            graphics.TranslateTransform(lowerRect.Left + lowerRect.Width / 2, lowerRect.Top + lowerRect.Height / 2);
        //            graphics.ScaleTransform(1, -1);
        //            graphics.DrawString(pkText, font, brush, 0, 0, sf);
        //            graphics.Restore(state);
        //        }

        //        using (var pen = new System.Drawing.Pen(System.Drawing.Color.DarkBlue, 1.5f))
        //            graphics.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        //    }
        //}



        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    var graphics = e.Graphics;
        //    var viewer = sender as Microsoft.Msagl.GraphViewerGdi.GViewer;

        //    if (viewer?.Graph == null)
        //        return;

        //    foreach (var node in viewer.Graph.Nodes)
        //    {
        //        // נניח שכל Node מחולק לשני חלקים:
        //        // חצי עליון = שם הטבלה (כחול כהה)
        //        // חצי תחתון = המפתח הראשי (קרם)

        //        // נשאב מיקום וגודל הצומת
        //        var boundingBox = node.BoundingBox;
        //        var rect = new System.Drawing.RectangleF(
        //            (float)boundingBox.Left,
        //            (float)boundingBox.Bottom,
        //            (float)boundingBox.Width,
        //            (float)boundingBox.Height);

        //        // מחלקים את המלבן לשני חלקים אנכית
        //        var upperRect = new System.Drawing.RectangleF(rect.Left, rect.Top, rect.Width, rect.Height / 2);
        //        var lowerRect = new System.Drawing.RectangleF(rect.Left, rect.Top + rect.Height / 2, rect.Width, rect.Height / 2);

        //        // צביעה של החלק העליון - כחול כהה
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.DarkBlue))
        //        {
        //            graphics.FillRectangle(brush, upperRect);
        //        }

        //        // צביעה של החלק התחתון - קרם (Bisque)
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Bisque))
        //        {
        //            graphics.FillRectangle(brush, lowerRect);
        //        }

        //        // רישום טקסטים:

        //        // טקסט עליון - שם הטבלה
        //        var tableName = node.LabelText?.Split('\n')[0] ?? "";

        //        using (var sf = new System.Drawing.StringFormat()
        //        {
        //            Alignment = System.Drawing.StringAlignment.Center,
        //            LineAlignment = System.Drawing.StringAlignment.Center
        //        })
        //        using (var font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
        //        {
        //            graphics.DrawString(tableName, font, brush, upperRect, sf);
        //        }

        //        // טקסט תחתון - מפתח ראשי (השארת שאר השורות אחרי השורה הראשונה)
        //        string pkText = "";
        //        if (!string.IsNullOrEmpty(node.LabelText))
        //        {
        //            var lines = node.LabelText.Split('\n');
        //            if (lines.Length > 1)
        //            {
        //                pkText = string.Join("\n", lines.Skip(1));
        //            }
        //        }

        //        using (var sf = new System.Drawing.StringFormat()
        //        {
        //            Alignment = System.Drawing.StringAlignment.Center,
        //            LineAlignment = System.Drawing.StringAlignment.Center
        //        })
        //        using (var font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Regular))
        //        using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
        //        {
        //            graphics.DrawString(pkText, font, brush, lowerRect, sf);
        //        }

        //        // ציור מסגרת סביב כל הצומת
        //        using (var pen = new System.Drawing.Pen(System.Drawing.Color.DarkBlue, 1.5f))
        //        {
        //            graphics.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
        //        }
        //    }
        //}





        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    if (_viewer.Graph == null || _viewer.Graph.GeometryGraph == null)
        //        return;

        //    var g = e.Graphics;

        //    using (var brush = new SolidBrush(System.Drawing.Color.LightSkyBlue))
        //    {
        //        foreach (var edge in _viewer.Graph.Edges)
        //        {
        //            var geomEdge = edge.GeometryEdge;
        //            if (geomEdge?.Curve == null)
        //                continue;

        //            // קצה הקו (נקודת הסיום)
        //            var target = geomEdge.Curve.End;

        //            // המרה לקואורדינטות מסך
        //            PointF screenPoint = GraphPointToScreen(target);

        //            // ציור עיגול בקצה הקו
        //            float radius = 2;
        //            g.FillEllipse(
        //                brush,
        //                screenPoint.X - radius,
        //                screenPoint.Y - radius,
        //                radius * 2,
        //                radius * 2
        //            );
        //        }
        //    }
        //}


        //private void Viewer_Paint(object sender, PaintEventArgs e)
        //{
        //    if (_viewer.Graph == null || _viewer.Graph.GeometryGraph == null)
        //        return;

        //    var g = e.Graphics;

        //    foreach (var node in _viewer.Graph.Nodes)
        //    {
        //        // חילוק טקסט לשורות
        //        var labelText = node.LabelText.Split(new[] { '\n' }, StringSplitOptions.None);

        //        // חישוב המיקום של הצומת
        //        var centerX = (float)node.GeometryNode.Center.X; // מרכז הצומת ב-X
        //        var centerY = (float)node.GeometryNode.Center.Y; // מרכז הצומת ב-Y

        //        // קביעת ממדי הצומת
        //        var width = 120;  // אורך הצומת
        //        var height = 60;  // גובה הצומת

        //        // קביעת מיקום הצומת
        //        var x = centerX - width / 2;
        //        var y = centerY - height / 2;

        //        // ציור הצומת עם תיקוני סיבוב
        //        using (var topBrush = new SolidBrush(System.Drawing.Color.DarkBlue)) // צבע כחול כהה לחלק העליון
        //        using (var bottomBrush = new SolidBrush(System.Drawing.Color.LemonChiffon)) // צבע בהיר לחלק התחתון
        //        {
        //            float topHeight = height / 2;  // חצי גובה לצורך החלק העליון
        //            var topRect = new RectangleF(x, y, width, topHeight);  // חלק עליון
        //            var bottomRect = new RectangleF(x, y + topHeight, width, height - topHeight); // חלק תחתון

        //            // צבעים של החלקים
        //            g.FillRectangle(topBrush, topRect);   // צבע כחול כהה בחלק העליון
        //            g.FillRectangle(bottomBrush, bottomRect);  // צבע בהיר בחלק התחתון

        //            // ציור של גבול שחור מסביב לצומת
        //            using (var pen = new Pen(System.Drawing.Color.Black, 2f)) // גבול שחור עבה
        //            {
        //                g.DrawRectangle(pen, x, y, width, height);  // גבול מסביב לצומת
        //            }

        //            // ציור טקסט בחלקים
        //            using (var font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)) // גופן לטקסט
        //            using (var textBrush = new SolidBrush(System.Drawing.Color.White)) // צבע טקסט לבן
        //            {
        //                // טקסט בחלק העליון (שם הטבלה)
        //                g.DrawString(labelText[0], font, textBrush, topRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.DirectionRightToLeft });

        //                // טקסט בחלק התחתון (מפתחות ראשיים)
        //                if (labelText.Length > 1)
        //                {
        //                    g.DrawString(labelText[1], font, textBrush, bottomRect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.DirectionRightToLeft });
        //                }
        //            }
        //        }
        //    }
        //}

        //זה טוב הלמטה
        //private PointF GraphPointToScreen(Microsoft.Msagl.Core.Geometry.Point graphPoint)
        //{
        //    // המרה ישירה, אם הגרף לא עובר זום או פאן, אפשר פשוט להמיר לנקודה במסך.
        //    // כאן אנחנו מניחים שהקואורדינטות במסך וגרף חופפים (עבור רוב המקרים).

        //    // הגישה הפשוטה: פשוט החזר את הקואורדינטות כפי שהן
        //    return new PointF((float)graphPoint.X, (float)graphPoint.Y);
        //}

        //private void BuildGraph(List<TableInfo> tables, List<RelationInfo> relations)
        //{
        //    var graph = new Microsoft.Msagl.Drawing.Graph();

        //    foreach (var table in tables)
        //    {
        //        string label = table.TableName;
        //        if (table.PrimaryKeys.Count > 0)
        //            label += "\nPK: " + string.Join(", ", table.PrimaryKeys);

        //        var node = graph.AddNode(table.TableName);
        //        node.LabelText = label;

        //        // פה Microsoft.Msagl.Drawing.Color חייבים - כי זה attr של msagl
        //        node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;

        //        node.Label.FontSize = 10;
        //        node.UserData = $"טבלה: {table.TableName}";

        //        node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Box;
        //        node.Attr.LineWidth = 1.5;
        //        node.Attr.Color = Microsoft.Msagl.Drawing.Color.Black;
        //        node.Attr.XRadius = 5;
        //        node.Attr.YRadius = 5;
        //        node.Label.FontName = "Segoe UI";
        //    }

        //    foreach (var rel in relations)
        //    {
        //        var edge = graph.AddEdge(rel.PrimaryTable, rel.ForeignTable);
        //        edge.Attr.ArrowheadAtTarget = Microsoft.Msagl.Drawing.ArrowStyle.Normal;
        //        edge.LabelText = rel.Cardinality;

        //        string description = "";
        //        if (rel.Cardinality == "1:N" || rel.Cardinality == "1:n")
        //            description = $"Each {rel.PrimaryTable} may have multiple {rel.ForeignTable.ToLower()}s";
        //        else if (rel.Cardinality == "1:1")
        //            description = $"Each {rel.PrimaryTable} has exactly one {rel.ForeignTable.ToLower()}";
        //        else if (rel.Cardinality == "0:1")
        //            description = $"Each {rel.ForeignTable} may have one {rel.PrimaryTable.ToLower()}";

        //        edge.UserData = description;
        //    }

        //    _viewer.Graph = graph;
        //    _viewer.Dock = DockStyle.Fill;

        //    if (!this.Controls.Contains(_viewer))
        //        this.Controls.Add(_viewer);

        //    _viewer.MouseMove -= Viewer_MouseMove;
        //    _viewer.MouseMove += Viewer_MouseMove;
        //}
*/
        /*        //    private void Viewer_MouseMove(object sender, MouseEventArgs e)
        //    {
        //        var viewer = sender as GViewer;
        //        if (viewer == null) return;

        //        var obj = viewer.GetObjectAt(e.Location);
        //        if (obj == null)
        //        {
        //            _customTooltip.Visible = false;
        //            return;
        //        }

        //        var prop = obj.GetType().GetProperty("DrawingObject");
        //        var drawingObject = prop?.GetValue(obj);

        //        if (drawingObject == null)
        //        {
        //            _customTooltip.Visible = false;
        //            return;
        //        }

        //        string tooltipText = "";

        //        if (drawingObject is Microsoft.Msagl.Drawing.Node node)
        //        {
        //            tooltipText = node.UserData?.ToString() ?? node.Id;
        //        }
        //        else if (drawingObject is Microsoft.Msagl.Drawing.Edge edge)
        //        {
        //            tooltipText = edge.UserData?.ToString() ?? $"{edge.Source} → {edge.Target}";
        //        }
        //        else
        //        {
        //            _customTooltip.Visible = false;
        //            return;
        //        }

        //        // רשימת מילים קבועות שלא יודגשו
        //        var fixedWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        //{
        //    "each", "may", "have", "multiple", "has", "exactly", "one", "to", "the", "of"
        //};

        //        // ניקוי הטקסט בטולטיפ
        //        _customTooltip.Clear();
        //        _customTooltip.Width = 250;
        //        _customTooltip.WordWrap = true;

        //        // פיצול המשפט למילים ושמירת הרווחים המקוריים
        //        var words = tooltipText.Split(' ');

        //        foreach (var word in words)
        //        {
        //            if (fixedWords.Contains(word.Trim('.', ',', ';')))
        //            {
        //                // טקסט קבוע - איטלקי בלבד
        //                _customTooltip.SelectionFont = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
        //                _customTooltip.SelectionColor = System.Drawing.Color.DarkBlue;
        //            }
        //            else
        //            {
        //                // טקסט דינמי - בולד + איטלקי
        //                _customTooltip.SelectionFont = new System.Drawing.Font("Segoe UI", 9,
        //                    System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
        //                _customTooltip.SelectionColor = System.Drawing.Color.DarkBlue;
        //            }

        //            _customTooltip.AppendText(word + " ");
        //        }

        //        // התאמת גובה
        //        _customTooltip.Height = _customTooltip.GetPositionFromCharIndex(_customTooltip.TextLength).Y + 20;

        //        // מיקום ליד העכבר
        //        var screenPos = viewer.PointToScreen(new System.Drawing.Point(e.X + 15, e.Y + 15));
        //        _customTooltip.Location = this.PointToClient(screenPos);
        //        _customTooltip.Visible = true;
        //        _customTooltip.BringToFront();
        //    }

        //private void Viewer_MouseMove(object sender, MouseEventArgs e)
        //{
        //    var viewer = sender as GViewer;
        //    if (viewer == null) return;

        //    var obj = viewer.GetObjectAt(e.Location);
        //    string tooltipText = null;

        //    if (obj != null)
        //    {
        //        var prop = obj.GetType().GetProperty("DrawingObject");
        //        var drawingObject = prop?.GetValue(obj);

        //        if (drawingObject is Microsoft.Msagl.Drawing.Node node)
        //        {
        //            tooltipText = node.UserData?.ToString() ?? $"Node: {node.Id}";
        //        }
        //        else if (drawingObject is Microsoft.Msagl.Drawing.Edge edge)
        //        {
        //            tooltipText = edge.UserData?.ToString() ?? $"Edge: {edge.Source} → {edge.Target}";
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(tooltipText))
        //    {
        //        _customTooltip.Text = tooltipText;
        //        var screenPos = viewer.PointToScreen(new System.Drawing.Point(e.X + 15, e.Y + 15));
        //        _customTooltip.Location = this.PointToClient(screenPos);
        //        _customTooltip.Visible = true;
        //    }
        //    else
        //    {
        //        _customTooltip.Visible = false;
        //    }
        //}

        //private void Viewer_MouseMove(object sender, MouseEventArgs e)
        //{
        //    var viewer = sender as GViewer;
        //    if (viewer == null) return;

        //    var obj = viewer.GetObjectAt(e.Location);
        //    string tooltipText = null;

        //    if (obj != null)
        //    {
        //        var prop = obj.GetType().GetProperty("DrawingObject");
        //        var drawingObject = prop?.GetValue(obj);

        //        if (drawingObject is Microsoft.Msagl.Drawing.Node node)
        //        {
        //            tooltipText = node.UserData?.ToString() ?? $"Node: {node.Id}";
        //        }
        //        else if (drawingObject is Microsoft.Msagl.Drawing.Edge edge)
        //        {
        //            tooltipText = edge.UserData?.ToString() ?? $"Edge: {edge.Source} → {edge.Target}";
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(tooltipText))
        //    {
        //        _customTooltip.Text = tooltipText;
        //        var screenPos = viewer.PointToScreen(new Point(e.X + 15, e.Y + 15));
        //        _customTooltip.Location = this.PointToClient(screenPos);
        //        _customTooltip.Visible = true;
        //    }
        //    else
        //    {
        //        _customTooltip.Visible = false;
        //    }
        //}
*/
        #endregion


    }
}
