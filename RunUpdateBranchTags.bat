
git subtree split --prefix=Assets/InstanceBrushTool --branch Release
git tags v0.1 Release
git push origin Release --tags
git branch -D Release