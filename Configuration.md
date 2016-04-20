# Introduction #

When TouchPal starts up it looks in it's home directory for a profile.  This profile is an xml file which describes all of the graphics to display and how to react to touch.


# Details #

TouchPal's default home directory is the TouchPal folder under your my documents directory.  When it starts up it will look for a profile with the filename of TouchPal.xml.  Both the home directory and profile filename can be changed by setting CommandLine switches.

TouchPal profiles structure is defined in the in the CockpitXML.xsd file packaged with the applicaiton.  The configuration file has four main sections:

  * Start Actions
  * Reset Actions
  * Controls
  * Layout

## Start Actions ##
Start actions are a set of ActionCommands executed when a new client game or applications connects with TouchPal.  Start actions are executed in the order in which they are defined in the file.
Example:
```
    <StartAction>NS:C,22,3008,1.0</StartAction>
    <StartAction>NS:C,22,3008,0.0</StartAction>
```


## Reset Actions ##
Reset actions are a set of ActionCommands which are executed when a reset is triggered.  A Reset will also reset all CockpitControls to their default values.
Example:
```
    <ResetAction>NS:R</ResetAction>
    <ResetAction>CV:5001=1</ResetAction>
```

## Controls ##
The ControlsElement contains all of the definitions of controls which can be displayed on the screen.  TouchPal currently supports Text and Button controls.
Example:
```
    <Controls>
    	<Text Name="WeaponsStoreType">
    	    <NetworkID>2000</NetworkID>
    	    <Width>40</Width>
    	    <Height>24</Height>
    	    <DefaultValue></DefaultValue>
    	    <FontFile>blackshark\digital-7 (mono).ttf</FontFile>
    	    <Font>Digital-7</Font>
    	    <FontSize>20</FontSize>
    	    <FontColor>
    	     	<Red>35</Red>
    	  	<Green>142</Green>
    	 	<Blue>98</Blue>
    	    </FontColor>
    	</Text>
	<Button Name="TargetShkInvert">
	    <NetworkID>404</NetworkID>
	    <Width>46</Width>
	    <Height>43</Height>
	    <DefaultValue>0.0</DefaultValue>
	    <State>
		<StateValue>0.0</StateValue>
		<Image>blackshark\target_shk_invert_off.png</Image>
		<PushedImage>blackshark\target_shk_invert_off_in.png</PushedImage>
		<PushedAction>NS:C,8,3001,1.0</PushedAction>
	    </State>
	    <State>
		<StateValue>1.0</StateValue>
		<Image>blackshark\target_shk_invert_on.png</Image>
		<PushedImage>blackshark\target_shk_invert_on_in.png</PushedImage>
		<PushedAction>NS:C,8,3001,0.0</PushedAction>
	    </State>
	</Button>
    </Controls>
```

## Layout ##
The LayoutElement defines panels which are groups of controls.  These definitions include the graphical layout of controls on to the panels and the location on the screen that these pannels are drawn.
Example:
```
   <Layout>
        <X>0</X>
        <Y>0</Y>
        <Width>1024</Width>
        <Height>768</Height>
        <BackgroundImage>blackshark\background.png</BackgroundImage>
        <TransparencyKey>
            <Red>255</Red>
	    <Green>0</Green>
	    <Blue>255</Blue>
	</TransparencyKey>

	<Panel Name="EKRAN">
	   <X>594</X>
	   <Y>0</Y>
	   <Width>174</Width>
	   <Height>198</Height>
	   <BackgroundImage>blackshark\panel_ekran_background.png</BackgroundImage>
	   <ControlLayout X="48" Y="94"  ControlName="EkranText1"/>
	   <ControlLayout X="48" Y="113" ControlName="EkranText2"/>
	   <ControlLayout X="48" Y="132" ControlName="EkranText3"/>
	   <ControlLayout X="48" Y="151" ControlName="EkranText4"/>
	</Panel>
    </Layout>
```