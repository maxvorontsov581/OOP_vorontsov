namespace OOP.Domain;

public delegate void LogisticsEvent<in T>(object sender, T e)
    where T : EventArgs;
