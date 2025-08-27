using System.Collections.Generic;

public class TargetResolver
{
    public List<Token> Resolve(BaseObject owner, Target target, EffectContext context)
    {
        TokenManager tokenManager = ServiceLocator.Get<TokenManager>(); 

        var list = new List<Token>();

        switch (target)
        {
            case Target.Self:

                break; 
        }

        return null; 
    }
}
