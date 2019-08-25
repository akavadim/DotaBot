using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotaBot.PS.IViews;
using DotaBot.BL.IModels;

namespace DotaBot.PS
{
    public class Presenter
    {
        readonly IMainWindow _mainWindow;
        readonly IMessageService _messageService;
        readonly IModel _model;

        public Presenter(IMainWindow mainWindow, IMessageService messageService, IModel model)
        {
            _mainWindow = mainWindow;
            _messageService = messageService;
            _model = model;
            
            //Обновление статусов
            DataBaseStatusUpdate();
            NotSortedDataUpdate();

            //Обработка событий MainWindow
            _mainWindow.AnalizDataClick += MainWindow_AnalizDataClick;
            _mainWindow.LoadFileDatabaseClick += MainWindow_PathDatabaseEdited;
            _mainWindow.LoadFileNotSortedDataClick += MainWindow_PathNotSortedDataEdited;
            _mainWindow.ParseClick += MainWindow_ParseClick;
            _mainWindow.ParsePageClick += MainWindow_ParsePageClick;
            _mainWindow.ParseMultyThreadClick += MainWindow_ParseMultyThreadClick;
            _mainWindow.ResultMatchClick += MainWindow_ResultMatchClick;
            _mainWindow.SaveFileDatabaseClick += MainWindow_SaveFileDatabaseClick;
            _mainWindow.SaveFileNotSortedDataClick += MainWindow_SaveFileNotSortedDataClick;
            _mainWindow.UpdateNotSortedDataClick += MainWindow_UpdateNotSortedDataClick;

            //Обработка событий Model
            _model.StatusDatabaseChanged += Model_StatusDatabaseChanged;
            _model.StatusNotSortedDataChanged += Model_StatusNotSortedDataChanged;
        }

        #region События MainWindow
        private void MainWindow_UpdateNotSortedDataClick(object sender, IStatusParse statusParse)
        {
            try
            {

                System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

                statusParse.CancelParseClick += (send, args) =>
                {
                    cancellationTokenSource.Cancel();
                };

                _model.ParserStatusUpdated += (send, args) =>
                {
                    statusParse.MatchesLoaded = args.MatchesLoaded.ToString();
                    statusParse.PagesOfMatchesLoaded = args.PagesOfMatchesLoaded.ToString();
                    statusParse.TotalLoaded = args.TotalLoaded.ToString();
                };

                _model.LoadPageError += (send, args) =>
                {
                    return _messageService.GetAnswer(args.Message + "\nПопробовать загрузить страницу еще раз?");
                };

                Task.Run(() =>
                {
                    _model.UppdateNotSortedData(cancellationTokenSource.Token);
                    statusParse.ParseEnded = true;
                });
            }
            catch(Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        private void MainWindow_ParseMultyThreadClick(object sender, IStatusParse statusParse)
        {
            try
            {
                System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

                statusParse.CancelParseClick += (send, args) =>
                {
                    cancellationTokenSource.Cancel();
                };

                _model.ParserStatusUpdated += (send, args) =>
                {
                    statusParse.MatchesLoaded = args.MatchesLoaded.ToString();
                    statusParse.PagesOfMatchesLoaded = args.PagesOfMatchesLoaded.ToString();
                    statusParse.TotalLoaded = args.TotalLoaded.ToString();
                };

                _model.LoadPageError += (send, args) =>
                {
                    return _messageService.GetAnswer(args.Message + "\nПопробовать загрузить страницу еще раз?");
                };

                Task.Run(() =>
                {
                    _model.ParseMultyThread(cancellationTokenSource.Token);
                    statusParse.ParseEnded = true;
                });
            }
            catch(Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
}

        private void MainWindow_SaveFileNotSortedDataClick(object sender, EventArgs e)
        {
            try
            {
                _model.SaveNotSortedData(_mainWindow.PathNotSortedData);
            }
            catch(Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        private void MainWindow_SaveFileDatabaseClick(object sender, EventArgs e)
        {
            try
            {
                _model.SaveDatabase(_mainWindow.PathDatabase);
            }
            catch(Exception ex)
            {
                _messageService.ShowMessage(ex.Message);
            }
        }

        private void MainWindow_ResultMatchClick(object sender, EventArgs e)
        {
            try
            {
                var resultMatch = _model.GetResultMatch(_mainWindow.LeftGamersUrls, _mainWindow.RightGamersUrls);
                string message = $"Моделирование сражения завершено.\n Шанс для левой команды: {resultMatch.Left}" +
                    $", правой команды: {resultMatch.Right}";
                _messageService.ShowMessage(message);
            }
            catch(Exception ex)
            { _messageService.ShowError(ex.Message); }
        }

        private void MainWindow_ParsePageClick(object sender, IStatusParse statusParse)
        {
            try
            {
                System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

                statusParse.CancelParseClick += (send, args) =>
                {
                    cancellationTokenSource.Cancel();
                };

                _model.ParserStatusUpdated += (send, args) =>
                {
                    statusParse.MatchesLoaded = args.MatchesLoaded.ToString();
                    statusParse.PagesOfMatchesLoaded = args.PagesOfMatchesLoaded.ToString();
                    statusParse.TotalLoaded = args.TotalLoaded.ToString();
                };

                _model.LoadPageError += (send, args) =>
                {
                    return _messageService.GetAnswer(args.Message + "\nПопробовать загрузить страницу еще раз?");
                };

                Task.Run(() =>
                {
                    _model.ParsePage(cancellationTokenSource.Token);
                    statusParse.ParseEnded = true;
                });
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        private void MainWindow_ParseClick(object sender, IStatusParse statusParse)
        {
            try
            {
                System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();

                statusParse.CancelParseClick += (send, args) =>
                {
                    cancellationTokenSource.Cancel();
                };

                _model.ParserStatusUpdated += (send, args) =>
                {
                    statusParse.MatchesLoaded = args.MatchesLoaded.ToString();
                    statusParse.PagesOfMatchesLoaded = args.PagesOfMatchesLoaded.ToString();
                    statusParse.TotalLoaded = args.TotalLoaded.ToString();
                };

                _model.LoadPageError += (send, args) =>
                  {
                      return _messageService.GetAnswer(args.Message + "\nПопробовать загрузить страницу еще раз?");
                  };

                Task.Run(() =>
                {
                    _model.Parse(cancellationTokenSource.Token);
                    statusParse.ParseEnded = true;
                });
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        private void MainWindow_PathNotSortedDataEdited(object sender, EventArgs e)
        {
            try
            {
                _model.LoadNotSortedData(_mainWindow.PathNotSortedData);
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        private void MainWindow_PathDatabaseEdited(object sender, EventArgs e)
        {
            try
            {
                _model.LoadDatabase(_mainWindow.PathDatabase);
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        private void MainWindow_AnalizDataClick(object sender, EventArgs e)
        {
            try
            {
                _model.AnalysisNotSortedData();
                _messageService.ShowMessage("Данные проанализированы");
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }
        #endregion

        #region События Model
        private void Model_StatusNotSortedDataChanged(object sender, EventArgs e)
        {
            NotSortedDataUpdate();
        }

        private void Model_StatusDatabaseChanged(object sender, EventArgs e)
        {
            DataBaseStatusUpdate();
        }
        #endregion

        private void DataBaseStatusUpdate()
        {
            if (_model.StatusDatabase)
                _mainWindow.DatabaseStatus = "Загружена";
            else _mainWindow.DatabaseStatus = "Не загружена";
        }

        private void NotSortedDataUpdate()
        {
            if (_model.StatusNotSortedData)
                _mainWindow.NotSortedDataStatus = "Загружены";
            else _mainWindow.NotSortedDataStatus = "Не загружены";
        }
    }
}
