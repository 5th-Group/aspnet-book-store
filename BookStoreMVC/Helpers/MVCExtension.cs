using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreMVC.Helpers;

public static class MvcExtensions
{
    private static readonly Func<string, char> SpecialCharacters = s =>
    {
        return s switch
        {
            "Space" => ' ',
            "Hyphen" => '-',
            "ForwardSlash" => '/',
            "BackSlash" => '\\',
            _ => new char()
        };
    };


    public static string ActiveClass(this IHtmlHelper htmlHelper, string controllers = null, string actions = null, string cssClass = "bg-primary text-white")
    {
        var currentController = htmlHelper?.ViewContext.RouteData.Values["controller"] as string;
        var currentAction = htmlHelper?.ViewContext.RouteData.Values["action"] as string;

        var acceptedControllers = (controllers ?? currentController ?? "").Split(',');
        var acceptedActions = (actions ?? currentAction ?? "").Split(',');

        return acceptedControllers.Contains(currentController) && acceptedActions.Contains(currentAction)
            ? cssClass
            : "";
    }

    public static string ToUrlFriendly(this string word)
    {
        var span = new List<char>();

        for (var i = 0; i < word.Length; i++){
            if (i != 0 && word[i] == ' ')
            {
                if (word[i + 1] < word.Length && word[i + 1] != SpecialCharacters("Hyphen")) span.Add(SpecialCharacters("Hyphen"));
            }
            else
            {
                span.Add(word[i]);
            }
        }
        
        return new string(span.ToArray()).ToLower();
    }
}