using UnityEngine;
using System;
using System.Collections;

namespace UI.UIComponent
{
    public class MultiPageController
    {
        public delegate void CurrentPageIndexChanged(int currentPageIndex, int oldPageIndex);

        public event CurrentPageIndexChanged OnPageIndexChanged;

        public MultiPageController(GameObject rootContent)
        {
            SetRootContent(rootContent);
        }


        public int TotalPageAmount()
        {
            if (rootPage != null)
            {
                return rootPage.transform.childCount;
            }

            return 0;
        }

        public void NextPage()
        {
            if (hasNextPage())
            {
                HidePage(CurrentPageIndex());
                ShowPage(CurrentPageIndex() + 1);
                SetCurrentPageIndex(CurrentPageIndex() + 1);
            }
        }

        public void PrevPage()
        {
            if (HasPrevPage())
            {
                HidePage(CurrentPageIndex());
                ShowPage(CurrentPageIndex() - 1);
                SetCurrentPageIndex(CurrentPageIndex() - 1);
            }
        }

        public int CurrentPageIndex()
        {
            return currentPageIndex;
        }

        public bool hasNextPage()
        {
            return CurrentPageIndex() < TotalPageAmount() - 1;
        }

        public bool HasPrevPage()
        {
            return CurrentPageIndex() > 0;
        }


        protected void HidePage(int pageIndex)
        {
            if (rootPage != null)
            {
                Transform pageTrans = rootPage.transform.Find(PAGE_PREFIX + pageIndex);
                if (pageTrans != null)
                {
                    pageTrans.gameObject.SetActive(false);
                }
            }
        }

        protected void ShowPage(int pageIndex)
        {
            if (rootPage != null)
            {
                Transform pageTrans = rootPage.transform.Find(PAGE_PREFIX + pageIndex);
                if (pageTrans != null)
                {
                    pageTrans.gameObject.SetActive(true);
                }
            }
        }

        protected void HideAllPages()
        {
            for (int pageIndex = 0; pageIndex < TotalPageAmount(); pageIndex++)
            {
                HidePage(pageIndex);
            }
        }

        protected void SetRootContent(GameObject rootPage)
        {
            this.rootPage = rootPage;
            if (TotalPageAmount() > 0)
            {
                HideAllPages();
                ShowPage(0);
            }
        }


        private void SetCurrentPageIndex(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= TotalPageAmount())
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            int oldPageIndex = currentPageIndex;
            currentPageIndex = pageIndex;
            if (OnPageIndexChanged != null)
            {
                OnPageIndexChanged(currentPageIndex, oldPageIndex);
            }
        }

        private GameObject rootPage;
        private int currentPageIndex;

        private readonly static string PAGE_PREFIX = "Page";
    }
}