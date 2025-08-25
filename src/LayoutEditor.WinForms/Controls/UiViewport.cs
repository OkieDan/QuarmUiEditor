using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LayoutEditor.Common;
using LayoutEditor.Common.Windows;

namespace LayoutEditor.WinForms.Controls
{
    public class UiViewport : Panel
    {
        private CharacterUiProfile _profile;
        private Size _targetResolution = new Size(2560, 1440);
        private float _scaleFactor = 1.0f;
        private bool _maintainAspectRatio = true;
        private Point _dragStartPoint;
        private UiWindowBase _selectedWindow;
        private bool _isDragging;
        private ResizeHandle _activeResizeHandle;
        
        // Cache for window rectangles to avoid recalculating during paint
        private Dictionary<string, Rectangle> _windowRectangles = new();
        
        public event EventHandler<UiWindowBase> SelectionChanged;

        public CharacterUiProfile Profile 
        { 
            get => _profile;
            set
            {
                _profile = value;
                RecalculateWindowRectangles();
                Invalidate();
            }
        }
        
        public Size TargetResolution
        {
            get => _targetResolution;
            set
            {
                _targetResolution = value;
                RecalculateWindowRectangles();
                Invalidate();
            }
        }
        
        public bool MaintainAspectRatio
        {
            get => _maintainAspectRatio;
            set
            {
                _maintainAspectRatio = value;
                RecalculateWindowRectangles();
                Invalidate();
            }
        }
        
        public UiViewport()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
            
            BackColor = Color.DarkGray;
            BorderStyle = BorderStyle.FixedSingle;
            
            // Setup events
            Resize += UiViewport_Resize;
            MouseDown += UiViewport_MouseDown;
            MouseMove += UiViewport_MouseMove;
            MouseUp += UiViewport_MouseUp;
        }
        
        private void UiViewport_Resize(object sender, EventArgs e)
        {
            RecalculateWindowRectangles();
            Invalidate();
        }
        
        private void RecalculateWindowRectangles()
        {
            _windowRectangles.Clear();
            
            if (_profile == null)
                return;
                
            string resolution = $"{_targetResolution.Width}x{_targetResolution.Height}";
            
            foreach (string windowName in _profile.WindowNames)
            {
                if (_profile.TryGetWindow<StandardWindow>(windowName, out var window))
                {
                    // Get position for this resolution
                    var position = window.GetPosition(resolution);
                    if (position != null)
                    {
                        int x = position.X;
                        int y = position.Y;
                        int width = window.Width;
                        int height = window.Height;
                        
                        // If width/height not specified, use default sizes
                        if (width <= 0) width = 100;
                        if (height <= 0) height = 100;
                        
                        _windowRectangles[windowName] = new Rectangle(x, y, width, height);
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (_profile == null)
                return;
                
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Calculate the maximum viewport size that fits within the control
            int availableWidth = ClientSize.Width;
            int availableHeight = ClientSize.Height;
            
            // Safety check - ensure we have some space to work with
            if (availableWidth <= 10 || availableHeight <= 10)
                return;
            
            float targetAspectRatio = (float)_targetResolution.Width / _targetResolution.Height;
            
            // Calculate viewport size while maintaining aspect ratio
            int viewportWidth, viewportHeight;
            
            if (_maintainAspectRatio)
            {
                // Use the available space efficiently while keeping aspect ratio
                if (availableWidth / targetAspectRatio <= availableHeight)
                {
                    // Width constrained
                    viewportWidth = availableWidth;
                    viewportHeight = (int)(availableWidth / targetAspectRatio);
                }
                else
                {
                    // Height constrained
                    viewportHeight = availableHeight;
                    viewportWidth = (int)(availableHeight * targetAspectRatio);
                }
                
                //// Apply a small reduction to ensure there's a margin
                //viewportWidth = (int)(viewportWidth * 0.95);
                //viewportHeight = (int)(viewportHeight * 0.95);
            }
            else
            {
                // Use most of the available space but leave a margin
                //viewportWidth = (int)(availableWidth * 0.95);
                //viewportHeight = (int)(availableHeight * 0.95);
                viewportWidth = availableWidth;
                viewportHeight = availableHeight;
            }

            // Calculate scale factor (consistent in both dimensions)
            _scaleFactor = Math.Min((float)viewportWidth / _targetResolution.Width, 
                                  (float)viewportHeight / _targetResolution.Height);
            
            // Ensure scale factor is reasonable
            if (_scaleFactor <= 0.01f)
                _scaleFactor = 0.01f;
            
            // Center the viewport in the available space
            int offsetX = (availableWidth - viewportWidth) / 2;
            int offsetY = (availableHeight - viewportHeight) / 2;
            
            // Remove this line that added extra margin at the top
            // offsetY += 10;
            
            // Ensure offsets are non-negative
            offsetX = Math.Max(0, offsetX);
            offsetY = Math.Max(0, offsetY);
            
            // Draw viewport background
            using (var brush = new SolidBrush(Color.FromArgb(30, 30, 30)))
            {
                g.FillRectangle(brush, offsetX, offsetY, viewportWidth, viewportHeight);
            }
            
            // Draw grid
            using (var pen = new Pen(Color.FromArgb(60, 60, 60)))
            {
                // Draw horizontal lines
                for (int y = 0; y <= _targetResolution.Height; y += 100)
                {
                    int scaledY = offsetY + (int)(y * _scaleFactor);
                    if (scaledY >= offsetY && scaledY <= offsetY + viewportHeight)
                        g.DrawLine(pen, offsetX, scaledY, offsetX + viewportWidth, scaledY);
                }
                
                // Draw vertical lines
                for (int x = 0; x <= _targetResolution.Width; x += 100)
                {
                    int scaledX = offsetX + (int)(x * _scaleFactor);
                    if (scaledX >= offsetX && scaledX <= offsetX + viewportWidth)
                        g.DrawLine(pen, scaledX, offsetY, scaledX, offsetY + viewportHeight);
                }
            }
            
            // Draw UI windows
            foreach (var kvp in _windowRectangles)
            {
                string windowName = kvp.Key;
                Rectangle rect = kvp.Value;
                
                // Scale and offset the rectangle
                Rectangle scaledRect = new Rectangle(
                    offsetX + (int)(rect.X * _scaleFactor),
                    offsetY + (int)(rect.Y * _scaleFactor),
                    Math.Max(1, (int)(rect.Width * _scaleFactor)),
                    Math.Max(1, (int)(rect.Height * _scaleFactor))
                );
                
                bool isSelected = _selectedWindow != null && _selectedWindow.Name == windowName;
                
                // Draw window rectangle
                using (var brush = new SolidBrush(isSelected ? Color.FromArgb(100, 100, 200, 100) : Color.FromArgb(80, 100, 100, 100)))
                {
                    g.FillRectangle(brush, scaledRect);
                }
                
                // Draw window border
                using (var pen = new Pen(isSelected ? Color.LimeGreen : Color.Gray))
                {
                    g.DrawRectangle(pen, scaledRect);
                }
                
                // Draw window title if there's enough space
                if (scaledRect.Width > 20 && scaledRect.Height > 10)
                {
                    using (var brush = new SolidBrush(Color.White))
                    using (var font = new Font("Segoe UI", 8f))
                    {
                        // Measure text to see if it fits
                        string displayText = windowName;
                        SizeF textSize = g.MeasureString(displayText, font);
                        
                        // If text is too wide, truncate it
                        if (textSize.Width > scaledRect.Width - 6)
                        {
                            displayText = windowName.Substring(0, Math.Max(1, windowName.Length * scaledRect.Width / (int)textSize.Width - 3)) + "...";
                        }
                        
                        g.DrawString(displayText, font, brush, 
                                    scaledRect.X + 3, 
                                    scaledRect.Y + 3);
                    }
                }
                
                // Draw resize handles if selected
                if (isSelected)
                {
                    DrawResizeHandles(g, scaledRect);
                }
            }
            
            // Draw resolution info
            using (var brush = new SolidBrush(Color.White))
            using (var font = new Font("Segoe UI", 9f))
            {
                string resInfo = $"Resolution: {_targetResolution.Width}x{_targetResolution.Height} - Scale: {_scaleFactor:F2}x";
                g.DrawString(resInfo, font, brush, 10, ClientSize.Height - 25);
            }
        }
        
        private void DrawResizeHandles(Graphics g, Rectangle rect)
        {
            int handleSize = 7;
            using (var brush = new SolidBrush(Color.White))
            using (var pen = new Pen(Color.Black))
            {
                // Draw the 8 resize handles
                // Top-left
                g.FillRectangle(brush, rect.Left - handleSize/2, rect.Top - handleSize/2, handleSize, handleSize);
                g.DrawRectangle(pen, rect.Left - handleSize/2, rect.Top - handleSize/2, handleSize, handleSize);
                
                // Top-middle
                g.FillRectangle(brush, rect.Left + rect.Width/2 - handleSize/2, rect.Top - handleSize/2, handleSize, handleSize);
                g.DrawRectangle(pen, rect.Left + rect.Width/2 - handleSize/2, rect.Top - handleSize/2, handleSize, handleSize);
                
                // Top-right
                g.FillRectangle(brush, rect.Right - handleSize/2, rect.Top - handleSize/2, handleSize, handleSize);
                g.DrawRectangle(pen, rect.Right - handleSize/2, rect.Top - handleSize/2, handleSize, handleSize);
                
                // Middle-left
                g.FillRectangle(brush, rect.Left - handleSize/2, rect.Top + rect.Height/2 - handleSize/2, handleSize, handleSize);
                g.DrawRectangle(pen, rect.Left - handleSize/2, rect.Top + rect.Height/2 - handleSize/2, handleSize, handleSize);
                
                // Middle-right
                g.FillRectangle(brush, rect.Right - handleSize/2, rect.Top + rect.Height/2 - handleSize/2, handleSize, handleSize);
                g.DrawRectangle(pen, rect.Right - handleSize/2, rect.Top + rect.Height/2 - handleSize/2, handleSize, handleSize);
                
                // Bottom-left
                g.FillRectangle(brush, rect.Left - handleSize/2, rect.Bottom - handleSize/2, handleSize, handleSize);
                g.DrawRectangle(pen, rect.Left - handleSize/2, rect.Bottom - handleSize/2, handleSize, handleSize);
                
                // Bottom-middle
                g.FillRectangle(brush, rect.Left + rect.Width/2 - handleSize/2, rect.Bottom - handleSize/2, handleSize, handleSize);
                g.DrawRectangle(pen, rect.Left + rect.Width/2 - handleSize/2, rect.Bottom - handleSize/2, handleSize, handleSize);
                
                // Bottom-right
                g.FillRectangle(brush, rect.Right - handleSize/2, rect.Bottom - handleSize/2, handleSize, handleSize);
                g.DrawRectangle(pen, rect.Right - handleSize/2, rect.Bottom - handleSize/2, handleSize, handleSize);
            }
        }
        
        private void UiViewport_MouseDown(object sender, MouseEventArgs e)
        {
            _dragStartPoint = e.Location;
            
            // Calculate viewport dimensions using same logic as in OnPaint
            int availableWidth = ClientSize.Width;
            int availableHeight = ClientSize.Height;
            float targetAspectRatio = (float)_targetResolution.Width / _targetResolution.Height;
            int viewportWidth, viewportHeight;
            
            if (_maintainAspectRatio)
            {
                if (availableWidth / targetAspectRatio <= availableHeight)
                {
                    viewportWidth = availableWidth;
                    viewportHeight = (int)(availableWidth / targetAspectRatio);
                }
                else
                {
                    viewportHeight = availableHeight;
                    viewportWidth = (int)(availableHeight * targetAspectRatio);
                }
            }
            else
            {
                viewportWidth = availableWidth;
                viewportHeight = availableHeight;
            }
            
            _scaleFactor = Math.Min((float)viewportWidth / _targetResolution.Width, 
                                  (float)viewportHeight / _targetResolution.Height);
            
            int offsetX = (availableWidth - viewportWidth) / 2;
            int offsetY = (availableHeight - viewportHeight) / 2;
            
            // Ensure offsets are non-negative
            offsetX = Math.Max(0, offsetX);
            offsetY = Math.Max(0, offsetY);
            
            // Save previous selection
            var previousSelection = _selectedWindow;
            
            // Check for resize handle first
            if (_selectedWindow != null)
            {
                Rectangle scaledRect = GetScaledRectangle(_selectedWindow.Name, offsetX, offsetY);
                _activeResizeHandle = HitTestResizeHandles(e.Location, scaledRect);
                
                if (_activeResizeHandle != ResizeHandle.None)
                {
                    _isDragging = true;
                    return;
                }
            }
            
            // Check for window selection
            _selectedWindow = null;
            foreach (var kvp in _windowRectangles)
            {
                Rectangle rect = kvp.Value;
                
                // Scale and offset the rectangle
                Rectangle scaledRect = new Rectangle(
                    offsetX + (int)(rect.X * _scaleFactor),
                    offsetY + (int)(rect.Y * _scaleFactor),
                    Math.Max(1, (int)(rect.Width * _scaleFactor)),
                    Math.Max(1, (int)(rect.Height * _scaleFactor))
                );
                
                if (scaledRect.Contains(e.Location))
                {
                    if (_profile?.TryGetWindow<StandardWindow>(kvp.Key, out var window) == true)
                    {
                        _selectedWindow = window;
                        _isDragging = true;
                        Invalidate();
                        break;
                    }
                }
            }
            
            // Raise event if selection changed
            if (_selectedWindow != previousSelection)
            {
                SelectionChanged?.Invoke(this, _selectedWindow);
            }
        }
        
        private void UiViewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _selectedWindow != null)
            {
                // Calculate viewport offset
                int viewportWidth = (int)(_targetResolution.Width * _scaleFactor);
                int viewportHeight = (int)(_targetResolution.Height * _scaleFactor);
                int offsetX = (Width - viewportWidth) / 2;
                int offsetY = (Height - viewportHeight) / 2;
                
                int deltaX = e.X - _dragStartPoint.X;
                int deltaY = e.Y - _dragStartPoint.Y;
                
                // Convert to target resolution coordinates
                int resolutionDeltaX = (int)(deltaX / _scaleFactor);
                int resolutionDeltaY = (int)(deltaY / _scaleFactor);
                
                string resolution = $"{_targetResolution.Width}x{_targetResolution.Height}";
                
                if (_activeResizeHandle != ResizeHandle.None)
                {
                    // Resize operation
                    if (_selectedWindow is StandardWindow stdWindow)
                    {
                        Rectangle rect = _windowRectangles[_selectedWindow.Name];
                        Rectangle newRect = rect;
                        
                        switch (_activeResizeHandle)
                        {
                            case ResizeHandle.TopLeft:
                                newRect.X += resolutionDeltaX;
                                newRect.Y += resolutionDeltaY;
                                newRect.Width -= resolutionDeltaX;
                                newRect.Height -= resolutionDeltaY;
                                break;
                            case ResizeHandle.TopMiddle:
                                newRect.Y += resolutionDeltaY;
                                newRect.Height -= resolutionDeltaY;
                                break;
                            case ResizeHandle.TopRight:
                                newRect.Y += resolutionDeltaY;
                                newRect.Width += resolutionDeltaX;
                                newRect.Height -= resolutionDeltaY;
                                break;
                            case ResizeHandle.MiddleLeft:
                                newRect.X += resolutionDeltaX;
                                newRect.Width -= resolutionDeltaX;
                                break;
                            case ResizeHandle.MiddleRight:
                                newRect.Width += resolutionDeltaX;
                                break;
                            case ResizeHandle.BottomLeft:
                                newRect.X += resolutionDeltaX;
                                newRect.Width -= resolutionDeltaX;
                                newRect.Height += resolutionDeltaY;
                                break;
                            case ResizeHandle.BottomMiddle:
                                newRect.Height += resolutionDeltaY;
                                break;
                            case ResizeHandle.BottomRight:
                                newRect.Width += resolutionDeltaX;
                                newRect.Height += resolutionDeltaY;
                                break;
                        }
                        
                        // Enforce minimum size
                        if (newRect.Width < 20) newRect.Width = 20;
                        if (newRect.Height < 20) newRect.Height = 20;
                        
                        // Update position
                        stdWindow.SetPosition(resolution, newRect.X, newRect.Y);
                        
                        // Update dimensions if they're supported
                        if (stdWindow is IDimensionable dimensionable)
                        {
                            dimensionable.SetWidth(newRect.Width);
                            dimensionable.SetHeight(newRect.Height);
                        }
                        
                        _windowRectangles[_selectedWindow.Name] = newRect;
                    }
                }
                else
                {
                    // Move operation
                    if (_selectedWindow is StandardWindow stdWindow)
                    {
                        var position = stdWindow.GetPosition(resolution);
                        if (position != null)
                        {
                            int newX = position.X + resolutionDeltaX;
                            int newY = position.Y + resolutionDeltaY;
                            
                            stdWindow.SetPosition(resolution, newX, newY);
                            
                            if (_windowRectangles.ContainsKey(_selectedWindow.Name))
                            {
                                Rectangle rect = _windowRectangles[_selectedWindow.Name];
                                rect.X = newX;
                                rect.Y = newY;
                                _windowRectangles[_selectedWindow.Name] = rect;
                            }
                        }
                    }
                }
                
                _dragStartPoint = e.Location;
                Invalidate();
            }
        }
        
        private void UiViewport_MouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
            _activeResizeHandle = ResizeHandle.None;
        }
        
        private Rectangle GetScaledRectangle(string windowName, int offsetX, int offsetY)
        {
            if (!_windowRectangles.TryGetValue(windowName, out var rect))
                return Rectangle.Empty;
                
            return new Rectangle(
                offsetX + (int)(rect.X * _scaleFactor),
                offsetY + (int)(rect.Y * _scaleFactor),
                Math.Max(1, (int)(rect.Width * _scaleFactor)),
                Math.Max(1, (int)(rect.Height * _scaleFactor))
            );
        }
        
        private ResizeHandle HitTestResizeHandles(Point location, Rectangle rect)
        {
            int handleSize = 7;
            
            // Top-left
            if (new Rectangle(rect.Left - handleSize/2, rect.Top - handleSize/2, handleSize, handleSize).Contains(location))
                return ResizeHandle.TopLeft;
                
            // Top-middle
            if (new Rectangle(rect.Left + rect.Width/2 - handleSize/2, rect.Top - handleSize/2, handleSize, handleSize).Contains(location))
                return ResizeHandle.TopMiddle;
                
            // Top-right
            if (new Rectangle(rect.Right - handleSize/2, rect.Top - handleSize/2, handleSize, handleSize).Contains(location))
                return ResizeHandle.TopRight;
                
            // Middle-left
            if (new Rectangle(rect.Left - handleSize/2, rect.Top + rect.Height/2 - handleSize/2, handleSize, handleSize).Contains(location))
                return ResizeHandle.MiddleLeft;
                
            // Middle-right
            if (new Rectangle(rect.Right - handleSize/2, rect.Top + rect.Height/2 - handleSize/2, handleSize, handleSize).Contains(location))
                return ResizeHandle.MiddleRight;
                
            // Bottom-left
            if (new Rectangle(rect.Left - handleSize/2, rect.Bottom - handleSize/2, handleSize, handleSize).Contains(location))
                return ResizeHandle.BottomLeft;
                
            // Bottom-middle
            if (new Rectangle(rect.Left + rect.Width/2 - handleSize/2, rect.Bottom - handleSize/2, handleSize, handleSize).Contains(location))
                return ResizeHandle.BottomMiddle;
                
            // Bottom-right
            if (new Rectangle(rect.Right - handleSize/2, rect.Bottom - handleSize/2, handleSize, handleSize).Contains(location))
                return ResizeHandle.BottomRight;
                
            return ResizeHandle.None;
        }
        
        private enum ResizeHandle
        {
            None,
            TopLeft,
            TopMiddle,
            TopRight,
            MiddleLeft,
            MiddleRight,
            BottomLeft,
            BottomMiddle,
            BottomRight
        }
    }
}