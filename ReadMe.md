﻿[![Build Status](https://dev.azure.com/Janco-Intl/Menominee/_apis/build/status%2FStaging%20Pipeline?branchName=MEN-1061-staging-slot-points-to-incorrect-api-url)](https://dev.azure.com/Janco-Intl/Menominee/_build/latest?definitionId=1&branchName=MEN-1061-staging-slot-points-to-incorrect-api-url)
# Menominee (StockTrac) Development
- [Coding Standard & Guidelines](CodeStandards.md)
- [Setting Up Development Environment](DevelopmentSetup.md)
- [Implementation Checklist](ImplementationChecklist.md)
- [Agile Scrum Guidelines](AgileScrum.md)
## Application Infrastructure:
- Jira holds the backlog and tasks
- Bitbucket hosts the code repository in two solutions: (TenantManager, Menominee)
  - The master branch is protected from commits and receives code changes by pull request (PR)
- Azure DevOps build pipelines
  - PR creation and updates build and (soon) test the source branch 
  - PR merges to master (or a target feature branch)
    - build and deploy the Staging slot for the API, Identity Server, Client Frontend
    - cache a copy of the idtempotent database build script in Azure Storage
- Azure
  - 3 slotted app services for the apps above
  - 2 databases: sample client stage, identity stage
- Tenant Manager console app - leverages the sql script generated by DevOps to set up tenants with a user and a tenant database
## Working on a Development Task
We use the [GitFlow](https://nvie.com/posts/a-successful-git-branching-model/) branching model. This means we have a master branch and a develop branch. We also use feature branches for development tasks.

![Git Hub Flow](../git-hub-flow.png)
### Start with Clean Functional Code
_definition:_ target branch - normally this is master but if you're working on a bigger set of changes it may be a special feature branch
1. Check your task in Jira and get the task ID number (formatted: MEN-###)
1. Move your task on the board into status "IN PROGRESS"
1. Check the task description for a definition of done so my 5th grader could test it out -- if it isn't clear bring it to the team for further grooming 
1. Check the build pipelines for the target branch are successful: [![Build Status](https://dev.azure.com/Janco-Intl/Menominee/_apis/build/status/Staging%20Pipeline?branchName=master)](https://dev.azure.com/Janco-Intl/Menominee/_build/latest?definitionId=1&branchName=master)
1. Git the latest target branch code (make sure it is "clean": `git pull` and `git reset --hard origin/target-branch`)
1. Create your working branch from target branch including the task ID number in the name ([Git Branch Naming](https://vimeo.com/813162599/cd70829ad5))
### Save Your Progress
Commit your work on a regular basis with real comments. Comments such as "responding to PR feedback" are less than useful. What actually changed in this commit makes a better comment and development trail.
### Test Your Work
If you didn't write unit tests along the way, add them now.

When you think you're done, actually go back to the "definition of done" on the task and do a full manual test.

If you've include EF Migrations consider if the changes are really what was expected: [Reference](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/managing?tabs=dotnet-core-cli)
### Read Your Code
Do a local compare to the target branch see all your code changes and check against the [General Guidelines & Standards](CodeStandards.md) and [Implementation Checklist](ImplementationChecklist.md) 

If you have any commented code, remove it unless it is actually useful for a future task or feature. 

If you decide to leave it in place:

1. comment it with a `// TODO` explaining why this code is useful
2. link it to a task by ID number (if needed create the task on the backlog)

### Prepare Your Pull Request (PR)

1. Git rebase your code on the target branch (you may want to do this before **Read Your Code**)
   
Note: if you need to do this manually use `git rebase -i origin/target-branch` keep the first commit and squash the following commits

2. You may need to force push your code after this: `git push --force`
1. Create a PR in BitBucket (the team should be automatically added as reviewers)
1. Actually read your code in BitBucket

Note: If there are multiple PRs with EF changes be careful of merge conflicts -- maintain the integrity of the model snapshot file: [Reference](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/teams)

5. Respond to feedback and rework as necessary
1. When you have the required two approvals, merge your code AND delete your branch 
1. Wait for the build success result
1. Test your changes in the target environment 
1. Move your task to status of "DONE"

Note: When merging a task into a target branch we prefer to use the squash method - this takes all the content from the task work and flattens it into one commit on the target branch.

When merging a feature or environment branch into another feature, environment, or master branch we use the rebase and fast forward method to maintain the history of the tasks that made up the feature. This is particularly helpful when using environment branches that progress from one another such as dev > stage > pre-production > production. A solid release pipeline can resolve the need to use environment branches.