using System;
using System.Collections.Generic;
using System.Linq;
using AllinaHealth.Models.Extensions;
using Sitecore.Data.Items;

namespace AllinaHealth.Models.ViewModels.HSG
{
    public class ArticleViewModel
    {
        private readonly Item _articleItem;
        private string _className;

        private string _topicClassName;

        //private string _titleName;
        private List<ArticleViewModel> _list;

        public ArticleViewModel(Item i)
        {
            _articleItem = i;
        }

        private List<ArticleViewModel> GetRelatedArticles()
        {
            //var list = new List<ArticleViewModel>();
            //var children = _articleItem.Parent.GetChildrenSafe();
            //children = children.Where(e => e.ID != Sitecore.Context.Item.ID).OrderByDescending<Item, string>(e => e.GetFieldValue("Article Posted Date")).Take(5).ToList();

            //list = children.ToList().Select(e => new ArticleViewModel(e)).ToList();
            //return list;

            //Comment the front-end section out before pushing the code to prod!
            //(Replace the below code with the code above)

            //Start of front-end code....
            var tempList = new List<ArticleViewModel>();
            var parentCategories = _articleItem.Parent.Parent.GetChildrenSafe();
            var artWithKeywordList = new List<Item>();

            foreach (var a in from c in parentCategories select c.GetChildrenSafe() into children select children.Where(e => e.ID != Sitecore.Context.Item.ID).ToList() into children from a in children where a.GetSelectedItems("Keyword").Any() && CurrentArticleKeywords.Count > 0 from kw in a.GetSelectedItems("Keyword") where CurrentArticleKeywords.Contains(kw.DisplayName) && !artWithKeywordList.Contains(a) select a)
            {
                artWithKeywordList.Add(a);
            }

            ////Some code for if we need to add more articles to a page
            ////var newestCCChildren = _articleItem.Parent.GetChildrenSafe(); //current category children (ccc)
            ////newestCCChildren = newestCCChildren.Where(e => e.ID != Sitecore.Context.Item.ID).OrderByDescending<Item, string>(e => e.GetFieldValue("Article Posted Date")).Take(5).ToList();
            ////artWithKeywordList = artWithKeywordList.Count > 0 ? artWithKeywordList.ToList() : newestCCChildren.ToList();


            //Original code, commented-out at least temporarily
            //tempList = artWithKeywordList.ToList().Select(e => new ArticleViewModel(e)).ToList();
            //list.AddRange(tempList);
            //var rnd = new Random();
            //int lc = list.Count > 5 ? 5 : list.Count;
            //for (int a = 0; a < lc; a++)
            //{
            //    int ra = a + rnd.Next(lc - a);
            //    ArticleViewModel t = list[ra];
            //    list[ra] = list[a];
            //    list[a] = t;
            //}

            //Test code for the front end below
            var list = artWithKeywordList.ToList().Select(e => new ArticleViewModel(e)).ToList();

            var rnd = new Random();
            var lc = list.Count > 5 ? 5 : list.Count;
            var rndList = new List<int>();
            for (var a = 0; a < lc; a++)
            {
                tempList.Add(null);
                var ra = rnd.Next(list.Count);
                if (!rndList.Contains(ra))
                {
                    rndList.Add(ra);
                    //throw new Exception("ra: " + ra + ". a: " + a);
                    tempList[a] = list[ra];
                }
                else
                {
                    a--;
                    //throw new Exception("Can Fail Too");
                }
            }

            list = tempList;
            //End of front-end code....

            return list;
        }

        public List<ArticleViewModel> RelatedArticles => _list ?? (_list = GetRelatedArticles());

        public string ClassName
        {
            get
            {
                if (string.IsNullOrEmpty(_className))
                {
                    _className = _articleItem.Parent.GetInternalLinkFieldItem("Category").GetFieldValue("Class Name").ToUpper();
                }

                return _className;
            }
        }

        public string TopicClassName
        {
            get
            {
                if (string.IsNullOrEmpty(_topicClassName))
                {
                    _topicClassName = _articleItem.Parent.GetInternalLinkFieldItem("Category").GetFieldValue("Topic Class Name");
                }

                return _topicClassName;
            }
        }

        //public string TitleName
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_titleName))
        //        {
        //            _titleName = _articleItem.GetFieldValue("Article Title");
        //        }
        //        return _titleName;
        //    }
        //}

        public Item ArticleItem => _articleItem;

        public List<string> CurrentArticleKeywords
        {
            get
            {
                var cakList = new List<string>();
                var currentArticleKeywords = Sitecore.Context.Item.GetSelectedItems("Keyword");
                foreach (var k in currentArticleKeywords)
                {
                    cakList.Add(k.DisplayName); //GetFieldValue("Keyword Name")
                }

                return cakList;
            }
        }
    }
}