using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LayoutEditor.Common;
using LayoutEditor.Common.Windows;

namespace LayoutEditor.WinForms.Controls
{
    public class UiViewport : Panel
    {
        // Make nullable with ? suffix
        private CharacterUiProfile? _profile;
        private Size _targetResolution = new Size(2560, 1440);
        private float _scaleFactor = 1.0f;
        private bool _maintainAspectRatio = true;
        private Point _dragStartPoint;
        private Point _currentMousePosition = Point.Empty;
        private UiWindowBase? _selectedWindow; // Make nullable
        private bool _isDragging;
        private ResizeHandle _activeResizeHandle;
        
        // New fields for drag selection
        private bool _isDragSelecting;
        private Rectangle _dragSelectRect;
        private List<UiWindowBase> _selectedWindows = new List<UiWindowBase>();
        
        // Cache for window rectangles to avoid recalculating during paint
        private Dictionary<string, Rectangle> _windowRectangles = new();
        
        // Make event nullable
        public event EventHandler<UiWindowBase?>? SelectionChanged;

        public CharacterUiProfile? Profile 
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

        // Make nullable
        public string? LoadedFilePath { get; internal set; }

        public UiViewport()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.Selectable,
                true);
            
            BackColor = Color.DarkGray;
            BorderStyle = BorderStyle.FixedSingle;
            TabStop = true;
            
            // Setup events - fix method signatures later
            Resize += UiViewport_Resize;
            MouseDown += UiViewport_MouseDown;
            MouseMove += UiViewport_MouseMove;
            MouseUp += UiViewport_MouseUp;
            KeyDown += UiViewport_KeyDown;
        }
        
        // Fix event handler signature with nullable
        private void UiViewport_Resize(object? sender, EventArgs e)
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
            }
            else
            {
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
                
                // Add support for multiple selection
                if (!isSelected && _selectedWindows.Any(w => w.Name == windowName))
                {
                    isSelected = true;
                }
                
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
                if (isSelected && _selectedWindow != null && _selectedWindow.Name == windowName)
                {
                    DrawResizeHandles(g, scaledRect);
                }
            }
            
            // Draw drag selection rectangle if active
            if (_isDragSelecting && !_dragSelectRect.IsEmpty)
            {
                using (var pen = new Pen(Color.White, 1))
                {
                    pen.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(pen, _dragSelectRect);
                }
                
                using (var brush = new SolidBrush(Color.FromArgb(30, 120, 180, 240)))
                {
                    g.FillRectangle(brush, _dragSelectRect);
                }
            }
            
            // Draw resolution info with mouse position
            using (var brush = new SolidBrush(Color.White))
            using (var font = new Font("Segoe UI", 9f))
            {
                // Calculate mouse position in target resolution coordinates
                Point mouseInTarget = Point.Empty;
                if (_currentMousePosition != Point.Empty && 
                    _currentMousePosition.X >= offsetX && 
                    _currentMousePosition.X < offsetX + viewportWidth &&
                    _currentMousePosition.Y >= offsetY && 
                    _currentMousePosition.Y < offsetY + viewportHeight)
                {
                    mouseInTarget = new Point(
                        (int)((_currentMousePosition.X - offsetX) / _scaleFactor),
                        (int)((_currentMousePosition.Y - offsetY) / _scaleFactor)
                    );
                }
                
                string resInfo = $"Resolution: {_targetResolution.Width}x{_targetResolution.Height} - Scale: {_scaleFactor:F2}x - Mouse: {mouseInTarget.X},{mouseInTarget.Y}";
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
        
        // Fix event handler signature
        private void UiViewport_MouseDown(object? sender, MouseEventArgs e)
        {
            // Ensure control gets focus when clicked
            Focus();
            
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
            var prevSelection = _selectedWindow;
            
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
            bool hitWindow = false;
            UiWindowBase? newSelection = null; // Make nullable
            
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
                    hitWindow = true;
                    if (_profile?.TryGetWindow<StandardWindow>(kvp.Key, out var window) == true)
                    {
                        newSelection = window;
                        
                        // Handle multi-select with Ctrl key
                        if (ModifierKeys.HasFlag(Keys.Control))
                        {
                            // Toggle selection state
                            if (_selectedWindows.Any(w => w.Name == window.Name))
                            {
                                _selectedWindows.RemoveAll(w => w.Name == window.Name);
                            }
                            else
                            {
                                _selectedWindows.Add(window);
                            }
                            
                            // Don't start dragging immediately with Ctrl
                            _isDragging = false;
                        }
                        else
                        {
                            // If not using Ctrl, clear other selections
                            if (!_selectedWindows.Any(w => w.Name == window.Name))
                            {
                                _selectedWindows.Clear();
                                _selectedWindows.Add(window);
                            }
                            
                            _isDragging = true;
                        }
                        
                        break;
                    }
                }
            }
            
            // If we hit a window, update the selection
            if (hitWindow)
            {
                _selectedWindow = newSelection;
                
                // Raise event if selection changed
                if (_selectedWindow != prevSelection)
                {
                    SelectionChanged?.Invoke(this, _selectedWindow);
                }
            }
            else
            {
                // If we didn't hit a window, start drag selection
                if (!ModifierKeys.HasFlag(Keys.Control))
                {
                    _selectedWindows.Clear();
                }
                
                _selectedWindow = null; // Now nullable, no error
                _isDragSelecting = true;
                _dragSelectRect = new Rectangle(e.Location, new Size(0, 0));
                
                // Raise event for cleared selection
                SelectionChanged?.Invoke(this, null); // Now using nullable event
            }
            
            Invalidate();
        }
        
        // Fix event handler signature
        private void UiViewport_MouseMove(object? sender, MouseEventArgs e)
        {
            _currentMousePosition = e.Location;
            
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
            }
            else if (_isDragSelecting)
            {
                // Update the drag selection rectangle
                int x = Math.Min(_dragStartPoint.X, e.X);
                int y = Math.Min(_dragStartPoint.Y, e.Y);
                int width = Math.Abs(e.X - _dragStartPoint.X);
                int height = Math.Abs(e.Y - _dragStartPoint.Y);
                
                _dragSelectRect = new Rectangle(x, y, width, height);
            }
            
            Invalidate();
        }
        
        // Fix event handler signature
        private void UiViewport_MouseUp(object? sender, MouseEventArgs e)
        {
            if (_isDragSelecting)
            {
                // Calculate viewport offset for coordinate conversion
                int viewportWidth = (int)(_targetResolution.Width * _scaleFactor);
                int viewportHeight = (int)(_targetResolution.Height * _scaleFactor);
                int offsetX = (Width - viewportWidth) / 2;
                int offsetY = (Height - viewportHeight) / 2;
                
                // Find all windows that intersect with the selection rectangle
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
                    
                    if (scaledRect.IntersectsWith(_dragSelectRect))
                    {
                        if (_profile?.TryGetWindow<UiWindowBase>(kvp.Key, out var window) == true)
                        {
                            if (!_selectedWindows.Any(w => w.Name == window.Name))
                            {
                                _selectedWindows.Add(window);
                                
                                // Set first selected window as the main selection
                                if (_selectedWindow == null)
                                {
                                    _selectedWindow = window;
                                    SelectionChanged?.Invoke(this, _selectedWindow);
                                }
                            }
                        }
                    }
                }
            }
            
            _isDragging = false;
            _isDragSelecting = false;
            _activeResizeHandle = ResizeHandle.None;
            _dragSelectRect = Rectangle.Empty;
            
            Invalidate();
        }
        
        // Fix event handler signature
        private void UiViewport_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (_profile != null)
                {
                    bool needRedraw = false;
                    
                    // Handle single selection
                    if (_selectedWindow != null)
                    {
                        // Remove window from the profile
                        _profile.RemoveWindow(_selectedWindow.Name);
                        
                        // Remove from the rectangles collection
                        _windowRectangles.Remove(_selectedWindow.Name);
                        
                        needRedraw = true;
                    }
                    
                    // Handle multi-selection
                    foreach (var window in _selectedWindows.ToList())
                    {
                        if (window != _selectedWindow) // Skip the main selection as it's already handled
                        {
                            _profile.RemoveWindow(window.Name);
                            _windowRectangles.Remove(window.Name);
                            needRedraw = true;
                        }
                    }
                    
                    if (needRedraw)
                    {
                        // Clear selection
                        _selectedWindow = null; // Now nullable, no error
                        _selectedWindows.Clear();
                        
                        // Notify selection changed
                        SelectionChanged?.Invoke(this, null); // Now nullable, no error
                        
                        // Redraw
                        Invalidate();
                        
                        e.Handled = true;
                    }
                }
            }
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