using System;
using System.Collections.Generic;

[Serializable]
public class DogApiResponse
{
    public List<DogBreedData> data;
    public Meta meta;
    public Links links;
}

[Serializable]
public class DogBreedData
{
    public string id;
    public string type;
    public BreedAttributes attributes;
    public BreedRelationships relationships;
}

[Serializable]
public class BreedAttributes
{
    public string name;
    public string description;
    public LifeSpan life;
    public Weight male_weight;
    public Weight female_weight;
    public bool hypoallergenic;
}

[Serializable]
public class LifeSpan
{
    public int max;
    public int min;
}

[Serializable]
public class Weight
{
    public int max;
    public int min;
}

[Serializable]
public class BreedRelationships
{
    public GroupData group;
}

[Serializable]
public class GroupData
{
    public GroupInfo data;
}

[Serializable]
public class GroupInfo
{
    public string id;
    public string type;
}

[Serializable]
public class Meta
{
    public Pagination pagination;
}

[Serializable]
public class Pagination
{
    public int current;
    public int next;
    public int last;
    public int records;
}

[Serializable]
public class Links
{
    public string self;
    public string current;
    public string next;
    public string last;
}

[Serializable]
public class DogBreed
{
    public string id;
    public string name;
    public string description;
    public string lifeSpan;
    public string weight;
    public bool hypoallergenic;

    public string SafeLifeSpan => string.IsNullOrEmpty(lifeSpan) ? "N/A" : lifeSpan;
    public string SafeWeight => string.IsNullOrEmpty(weight) ? "N/A" : weight;
}