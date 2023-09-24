export const hair_shader = {
  label: "hair shader",
  code: `
        struct VertexOut{
          @builtin(position) position : vec4f,
          @location(0) color : vec4f,
        }

        @group(0) @binding(0) var<uniform> transform: mat4x4<f32>;
        @group(0) @binding(1) var<uniform> screen: vec2f;        

        @vertex
        fn vertexMain(@location(0) position: vec2f, @location(1) color: vec4f) -> VertexOut {
          // dummy so auto layout sees the variable 
          let dummy = screen;
          return VertexOut(transform*vec4f(position, 0, 1), color);                  
        }         

        @fragment
        fn fragmentMain(vertexOut: VertexOut) -> @location(0) vec4f {
          return vertexOut.color;
        }
      `
};

export const fastline_shader = {
  label: "line shader",
  code: `
        @group(0) @binding(0) var<uniform> transform: mat4x4<f32>;
        @group(0) @binding(1) var<uniform> screen: vec2f;

        struct VertexOut{
          @builtin(position) position : vec4f,
          @location(0) color : vec4f,
        }

        @vertex
        fn vertexMain(@location(0) A: vec2f, @location(1) B: vec2f, @location(2) C: vec2f,@location(3) colorAndDir: vec4f)
          -> VertexOut {                             

          // thickness in pixel
          let thickness = 1.0;               
          let uscreen = vec2f(1, screen.y/screen.x);       
          // half of linewidth in normal space (= -1 to 1)
          let uwidth2 = thickness*1.0/screen.x;

          var a = (transform*vec4f(A, 0, 1)).xy;
          let b = (transform*vec4f(B, 0, 1)).xy;
          var c = (transform*vec4f(C, 0, 1)).xy;
          if(length(b-a)==0)
            {a = b + normalize(b-c);}
          if(length(b-c)==0)
            {c = b + normalize(b-a);}
          let ab = normalize(normalize((b-a))*uscreen);
          let bc = normalize(normalize((c-b))*uscreen);
          let tangent = normalize(ab+bc);
          let miter = vec2f(-tangent.y,tangent.x);
          let normalA = vec2f(-ab.y,ab.x);
          let miterLength = 1 / dot(miter, normalA);
          return VertexOut(vec4f(b+sign(colorAndDir.w)*uwidth2*miter*miterLength/uscreen , 0, 1), colorAndDir);          
        }         

        @fragment
        fn fragmentMain(vertexOut: VertexOut) -> @location(0) vec4f {
          return vec4f(vertexOut.color.xyz,1);
        }
      `
};

export const aa_fastline_shader = {
  label: "line shader with aa",
  code: `
        @group(0) @binding(0) var<uniform> xTransform: mat4x4<f32>;
        @group(0) @binding(1) var<uniform> screen: vec2f;
        @group(0) @binding(2) var<storage> signals: array<SignalAttributes>;

        struct SignalAttributes{
          color : vec4f,
          // structs must have a size equal to a multiple of 16
          // the @size(16) is actually only relevant if there is another variable
          @size(16)  thickness : f32,
          transform : mat4x4<f32>,
        }

        struct VertexOut{
          @builtin(position) position : vec4f,
          @location(0)@interpolate(flat) index : i32,
          // distance of the vertex from the line in screen space 
          @location(1)@interpolate(linear) screenDist : vec2f,
        }

        // signedIndex is a 2d float, x corresponds to the index of the signal and y is -1 or 1 determining in what direction the vertex has to go
        @vertex
        fn vertexMain(@location(0) A: vec2f, @location(1) B: vec2f, @location(2) C: vec2f, @location(3) signedIndex: vec2f)
          -> VertexOut {                             
          
          let index = i32(signedIndex.x);
          let signal = signals[index];
          // line radius in pixel but wider, because the purpose is only to capture enough pixel so the fragment shader is invoked on them
          let thickness = signal.thickness + 10;
          let radius = thickness/2;
          let uscreen = vec2f(1, screen.y/screen.x);       
          let uwidth = thickness*1.0/screen.x;
          let dir = select(1.0,-1.0,signedIndex.y<0);

          let transform = xTransform * signal.transform;
          var a = (transform*vec4f(A, 0, 1)).xy;
          let b = (transform*vec4f(B, 0, 1)).xy;
          var c = (transform*vec4f(C, 0, 1)).xy;
          if(length(b-a)==0)
            {a = b + normalize(b-c);}
          if(length(b-c)==0)
            {c = b + normalize(b-a);}
          let ab = normalize(normalize((b-a))*uscreen);
          let bc = normalize(normalize((c-b))*uscreen);
          let tangent = normalize(ab+bc);
          let miter = vec2f(-tangent.y,tangent.x);
          let normalA = vec2f(-ab.y,ab.x);
          let miterLength = 1 / dot(miter, normalA);   
                
          // distance of vertex to the line in pixelspace/screenspace
          // fragment shaders is rendered at the center of each pixel
          // by using interpolation the fragment shader knows the distance to the line in screenspace
          let screenDist = vec2f(dir*radius,0);
          
          return VertexOut(vec4f(b+dir*uwidth*miter*miterLength/uscreen , 0, 1), index, screenDist);          
        }         

        @fragment
        fn fragmentMain(in: VertexOut) -> @location(0) vec4f {
          let radius = signals[in.index].thickness/2.0;
          let smoothSize = 0.5;
          var x = abs(in.screenDist.x);
          var a = 1-smoothstep(radius - smoothSize, radius + smoothSize, x);                                     
          return vec4f(signals[in.index].color.xyz, a);
        }
      `
};