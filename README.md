# TriggerIt

TriggerIt is a lightweight library for triggering actions based on logged events or uptime. It's origin is in Xamarin Forms development but it can be easily extended.

## How to

### Init

You need this Nuget:

```
TriggerIt
```

For Xamarin Forms, you need this one:

```
TriggerIt.Xamarin.Forms
```

### Use in Xamarin Forms

1. Initialize TriggerIt service in App.cs:

```csharp
await TriggerIt.Xamarin.Forms.Service.InitAsync();
```

2. Create a trigger class:

```csharp
public class FeedbackTrigger : TriggerIt.Triggers.Trigger
{
	public override string Name => nameof(FeedbackTrigger);

	public override async Task ExecuteAsync(TriggerContext context)
	{
		await App.Current.MainPage.DisplayAlert("Help", "Send us feedback!", "Later", "OK");
	}
}
```

3. Configure trigger planning:

```csharp
TriggerIt.Xamarin.Forms.Service.Instance.RegisterTrigger(new FeedbackTrigger(),
                TriggerPlanning.If()
                        .EventCountSinceUptimeIs("my_event_name", /* events count */ 3)
                        .UptimeIsAtLeast(TimeSpan.FromSeconds(30)));
```

4. Log events (on button click events or similar):

```csharp
await TriggerIt.Xamarin.Forms.Service.Instance.LogEventAsync("my_event_name");
```

5. When FeedbackTrigger meets plan conditions, `ExecuteAsync` method is executed.