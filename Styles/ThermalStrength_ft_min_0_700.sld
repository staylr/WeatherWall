<?xml version="1.0" encoding="ISO-8859-1"?>
<StyledLayerDescriptor version="1.0.0" 
 xsi:schemaLocation="http://www.opengis.net/sld StyledLayerDescriptor.xsd" 
 xmlns="http://www.opengis.net/sld" 
 xmlns:ogc="http://www.opengis.net/ogc" 
 xmlns:xlink="http://www.w3.org/1999/xlink" 
 xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <!-- a Named Layer is the basic building block of an SLD document -->
  <NamedLayer>
    <Name>RASP_WSTAR</Name>
    <UserStyle>
    <!-- Styles can have names, titles and abstracts -->
      <Title>Thermal Updraft Strength ft/min</Title>
      <Abstract></Abstract>
      <FeatureTypeStyle>
        <Rule>
          <Name>WStar_Colour</Name>
          <Title>8 Colour Red Map</Title>
          <Abstract></Abstract>
          <RasterSymbolizer>
            <Opacity>1.0</Opacity>
            <ColorMap extended="true">
			  <ColorMapEntry color="#ffffff" quantity="10" opacity="0" />
              <ColorMapEntry color="#ffffcc" quantity="100" />
              <ColorMapEntry color="#ffeda0" quantity="200" />
              <ColorMapEntry color="#fed976" quantity="300" />
              <ColorMapEntry color="#feb24c" quantity="400" />
              <ColorMapEntry color="#fd8d3c" quantity="500" />
              <ColorMapEntry color="#fc4e2a" quantity="600" />
              <ColorMapEntry color="#e31a1c" quantity="700" />
              <ColorMapEntry color="#b10026" quantity="5000" />
            </ColorMap>
          </RasterSymbolizer>
        </Rule>
      </FeatureTypeStyle>
    </UserStyle>
  </NamedLayer>
</StyledLayerDescriptor>
