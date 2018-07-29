using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTGOPrices
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region "INPC"
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string memberName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }

        public void NotifyPropertyChanged<T>(ref T fieldValue, T value, [CallerMemberName] string memberName = null)
        {
            fieldValue = value;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }
        }
        #endregion

        private string deckUri;
        public string DeckUri
        {
            get
            {
                return deckUri;
            }

            set
            {
                NotifyPropertyChanged(ref deckUri, value);
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<Card> cardList;
        public System.Collections.ObjectModel.ObservableCollection<Card> CardList
        {
            get
            {
                return cardList;
            }

            set
            {
                NotifyPropertyChanged(ref cardList, value);
            }
        }

        private bool processing;
        public bool Processing
        {
            get
            {
                return processing;
            }

            set
            {
                NotifyPropertyChanged(ref processing, value);
            }
        }

        private bool isIndeterminate;
        public bool IsIndeterminate
        {
            get
            {
                return isIndeterminate;
            }

            set
            {
                NotifyPropertyChanged(ref isIndeterminate, value);
            }
        }

        private int progress;
        public int Progress
        {
            get
            {
                return progress;
            }

            set
            {
                NotifyPropertyChanged(ref progress, value);
            }
        }

        private int maxProgress;
        public int MaxProgress
        {
            get
            {
                return maxProgress;
            }

            private set
            {
                NotifyPropertyChanged(ref maxProgress, value);
            }
        }

        private string progressMessage;
        public string ProgressMessage
        {
            get
            {
                return progressMessage;
            }

            private set
            {
                NotifyPropertyChanged(ref progressMessage, value);
            }
        }

        public ICommand ShowPricesCommand
        {
            get
            {
                return new MVVM.DelegateCommand(ShowPrices);
            }
        }

        public MainWindowViewModel()
        {
            CardList = new System.Collections.ObjectModel.ObservableCollection<Card>();
            DeckUri = "https://www.cardhoarder.com/d/5b30f9c57ed61";
        }

        private async void ShowPrices()
        {
            ProgressMessage = "Loading decklist ...";
            IsIndeterminate = true;
            Processing = true;
            string html = RequestHandler.GetContent(DeckUri);
            List<Card> allCards = RequestHandler.GetCards(html).ToList();
            IsIndeterminate = false;
            MaxProgress = allCards.Count();
            for (int i = 0; i < allCards.Count(); i++)
            {
                ProgressMessage = String.Format("Fetching card prices ({0}/{1})", i + 1, MaxProgress);
                Card card = allCards[i];
                card.Uri = String.Format(RequestHandler.GoatUrl, card.Name.Replace(" ", "-").Replace("'", String.Empty)).ToLower();
                RequestHandler.SetLowestPrice(card);
                CardList.Add(card);
                Progress = CardList.Count;

                if (i == allCards.Count - 1)
                {
                    Processing = false;
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}
