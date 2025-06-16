namespace UnityHeapCrawler;

public enum CrawlOrder {
	UserRoots,
	StaticFields,
	Hierarchy,
	SriptableObjects,
	Prefabs,
	UnityObjects
}